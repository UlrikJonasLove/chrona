using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimeEntries.Delete;

public record DeleteTimeEntryCommand(long TimeEntryId)
	: IRequest;

public class DeleteTimeEntryCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<DeleteTimeEntryCommand>
{
	public async Task Handle(DeleteTimeEntryCommand command, CancellationToken cancellationToken)
	{
		var timeEntryToDelete = await context.TimeEntries
			.AsTracking()
			.Include(timeEntry => timeEntry.TimesheetWeek)
			.FirstAsync(timeEntry => timeEntry.Id == command.TimeEntryId, cancellationToken);

		if (timeEntryToDelete.TimesheetWeek.Status is TimesheetWeekStatus.Submitted or TimesheetWeekStatus.Approved)
			throw new InvalidOperationException("The timesheet week is locked and cannot be modified.");

		context.TimeEntries.Remove(timeEntryToDelete);

		timeEntryToDelete.TimesheetWeek.UpdatedAtUtc = DateTime.UtcNow;

		await context.SaveChangesAsync(cancellationToken);
	}
}
