using System.Globalization;
using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimeEntries.Commands.Create;

public record CreateTimeEntryCommand(
	long UserId,
	long ProjectId,
	DateOnly WorkDate,
	int Minutes,
	string? Comment,
	bool StartNow = false,
	DateTime? StartedAtUtc = null,
	DateTime? StoppedAtUtc = null)
	: IRequest<long>;

public class CreateTimeEntryCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<CreateTimeEntryCommand, long>
{
	public async Task<long> Handle(CreateTimeEntryCommand command, CancellationToken cancellationToken)
	{
		if (command.Minutes < 0 || command.Minutes > 1440)
			throw new InvalidOperationException("Minutes must be between 0 and 1440.");

		var nowUtc = DateTime.UtcNow;
		var startedAtUtc = command.StartNow ? nowUtc : command.StartedAtUtc;
		var stoppedAtUtc = command.StoppedAtUtc;

		var hasAnyTimerValue = startedAtUtc is not null || stoppedAtUtc is not null;

		if (stoppedAtUtc is not null && startedAtUtc is null)
			throw new InvalidOperationException("StoppedAtUtc cannot be set without StartedAtUtc.");

		if (startedAtUtc is not null && stoppedAtUtc is not null && stoppedAtUtc < startedAtUtc)
			throw new InvalidOperationException("StoppedAtUtc cannot be earlier than StartedAtUtc.");

		var minutesToStore = command.Minutes;

		if (startedAtUtc is not null && stoppedAtUtc is not null && minutesToStore == 0)
		{
			var elapsedMinutes = (int)Math.Floor((stoppedAtUtc.Value - startedAtUtc.Value).TotalMinutes);
			if (elapsedMinutes < 0)
				throw new InvalidOperationException("Calculated minutes cannot be negative.");

			minutesToStore = elapsedMinutes;
		}

		if (startedAtUtc is not null && stoppedAtUtc is null && minutesToStore != 0)
			throw new InvalidOperationException("Minutes must be 0 for an ongoing time entry.");

		if (!hasAnyTimerValue && minutesToStore == 0)
			throw new InvalidOperationException("Minutes must be greater than 0 for a manual time entry.");

		var workDateTime = command.WorkDate.ToDateTime(TimeOnly.MinValue);
		var isoYear = ISOWeek.GetYear(workDateTime);
		var isoWeek = ISOWeek.GetWeekOfYear(workDateTime);

		var timesheetWeek = await context.TimesheetWeeks
			.AsTracking()
			.SingleOrDefaultAsync(
				timesheetWeekEntity =>
					timesheetWeekEntity.UserId == command.UserId &&
					timesheetWeekEntity.IsoYear == isoYear &&
					timesheetWeekEntity.IsoWeek == isoWeek,
				cancellationToken);

		if (timesheetWeek is null)
		{
			var timesheetWeekToCreate = new TimesheetWeek
			{
				UserId = command.UserId,
				IsoYear = isoYear,
				IsoWeek = isoWeek,
				Status = TimesheetWeekStatus.Draft,
				CreatedAtUtc = nowUtc,
				UpdatedAtUtc = nowUtc
			};

			context.TimesheetWeeks.Add(timesheetWeekToCreate);
			timesheetWeek = timesheetWeekToCreate;
		}

		if (timesheetWeek.Status is TimesheetWeekStatus.Submitted or TimesheetWeekStatus.Approved)
			throw new InvalidOperationException("The timesheet week is locked and cannot be modified.");

		var timeEntryToCreate = new TimeEntry
		{
			TimesheetWeek = timesheetWeek,
			UserId = command.UserId,
			ProjectId = command.ProjectId,
			WorkDate = command.WorkDate,
			Minutes = minutesToStore,
			StartedAtUtc = startedAtUtc,
			StoppedAtUtc = stoppedAtUtc,
			Comment = command.Comment,
			CreatedAtUtc = nowUtc,
			UpdatedAtUtc = nowUtc
		};

		context.TimeEntries.Add(timeEntryToCreate);

		timesheetWeek.UpdatedAtUtc = nowUtc;

		await context.SaveChangesAsync(cancellationToken);

		return timeEntryToCreate.Id;
	}
}
