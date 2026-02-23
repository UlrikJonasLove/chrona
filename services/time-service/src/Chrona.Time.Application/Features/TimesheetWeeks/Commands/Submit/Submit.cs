using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimesheetWeeks.Submit;

public record SubmitTimesheetWeekCommand(
	long TimesheetWeekId,
	long SubmittedByUserId)
	: IRequest;

public class SubmitTimesheetWeekCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<SubmitTimesheetWeekCommand>
{
	public async Task Handle(SubmitTimesheetWeekCommand command, CancellationToken cancellationToken)
	{
		var timesheetWeekToSubmit = await context.TimesheetWeeks
			.AsTracking()
			.FirstAsync(timesheetWeek => timesheetWeek.Id == command.TimesheetWeekId, cancellationToken);

		if (timesheetWeekToSubmit.Status is TimesheetWeekStatus.Submitted)
			return;

		if (timesheetWeekToSubmit.Status is not TimesheetWeekStatus.Draft)
			throw new InvalidOperationException("Only draft timesheet weeks can be submitted.");

		var hasAnyTimeEntries = await context.TimeEntries
			.AsNoTracking()
			.AnyAsync(timeEntry => timeEntry.TimesheetWeekId == timesheetWeekToSubmit.Id, cancellationToken);

		if (!hasAnyTimeEntries)
			throw new InvalidOperationException("A timesheet week cannot be submitted without any time entries.");

		var hasOngoingTimeEntries = await context.TimeEntries
			.AsNoTracking()
			.AnyAsync(
				timeEntry =>
					timeEntry.TimesheetWeekId == timesheetWeekToSubmit.Id &&
					timeEntry.StartedAtUtc != null &&
					timeEntry.StoppedAtUtc == null,
				cancellationToken);

		if (hasOngoingTimeEntries)
			throw new InvalidOperationException("A timesheet week cannot be submitted while it has ongoing time entries.");

		var nowUtc = DateTime.UtcNow;
		var previousStatus = timesheetWeekToSubmit.Status;

		timesheetWeekToSubmit.Status = TimesheetWeekStatus.Submitted;
		timesheetWeekToSubmit.SubmittedAtUtc = nowUtc;
		timesheetWeekToSubmit.SubmittedByUserId = command.SubmittedByUserId;
		timesheetWeekToSubmit.UpdatedAtUtc = nowUtc;

		var statusEventToCreate = new TimesheetWeekStatusEvent
		{
			TimesheetWeekId = timesheetWeekToSubmit.Id,
			FromStatus = previousStatus,
			ToStatus = TimesheetWeekStatus.Submitted,
			ChangedAtUtc = nowUtc,
			ChangedByUserId = command.SubmittedByUserId,
			Note = null
		};

		context.TimesheetWeekStatusEvents.Add(statusEventToCreate);

		await context.SaveChangesAsync(cancellationToken);
	}
}
