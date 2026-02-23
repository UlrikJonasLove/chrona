using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimeEntries.Update;

public record UpdateTimeEntryCommand(
	long TimeEntryId,
	int Minutes,
	string? Comment)
	: IRequest;

public class UpdateTimeEntryCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<UpdateTimeEntryCommand>
{
	public async Task Handle(UpdateTimeEntryCommand command, CancellationToken cancellationToken)
	{
		if (command.Minutes < 0 || command.Minutes > 1440)
			throw new InvalidOperationException("Minutes must be between 0 and 1440.");

		var timeEntryToUpdate = await context.TimeEntries
			.AsTracking()
			.Include(timeEntry => timeEntry.TimesheetWeek)
			.FirstAsync(timeEntry => timeEntry.Id == command.TimeEntryId, cancellationToken);

		if (timeEntryToUpdate.TimesheetWeek.Status is TimesheetWeekStatus.Submitted or TimesheetWeekStatus.Approved)
			throw new InvalidOperationException("The timesheet week is locked and cannot be modified.");

		if (timeEntryToUpdate.StartedAtUtc is not null && timeEntryToUpdate.StoppedAtUtc is null)
			throw new InvalidOperationException("An ongoing time entry cannot be updated. Stop it first.");

		timeEntryToUpdate.Minutes = command.Minutes;
		timeEntryToUpdate.Comment = command.Comment;
		timeEntryToUpdate.UpdatedAtUtc = DateTime.UtcNow;

		timeEntryToUpdate.TimesheetWeek.UpdatedAtUtc = DateTime.UtcNow;

		await context.SaveChangesAsync(cancellationToken);
	}
}
