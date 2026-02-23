using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimesheetWeeks.Reopen;

public record ReopenTimesheetWeekCommand(
	long TimesheetWeekId,
	long ReopenedByUserId,
	string? Note)
	: IRequest;

public class ReopenTimesheetWeekCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<ReopenTimesheetWeekCommand>
{
	public async Task Handle(ReopenTimesheetWeekCommand command, CancellationToken cancellationToken)
	{
		var timesheetWeekToReopen = await context.TimesheetWeeks
			.AsTracking()
			.FirstAsync(timesheetWeek => timesheetWeek.Id == command.TimesheetWeekId, cancellationToken);

		if (timesheetWeekToReopen.Status is TimesheetWeekStatus.Draft)
			return;

		if (timesheetWeekToReopen.Status is not TimesheetWeekStatus.Rejected)
			throw new InvalidOperationException("Only rejected timesheet weeks can be reopened.");

		var nowUtc = DateTime.UtcNow;
		var previousStatus = timesheetWeekToReopen.Status;

		timesheetWeekToReopen.Status = TimesheetWeekStatus.Draft;
		timesheetWeekToReopen.SubmittedAtUtc = null;
		timesheetWeekToReopen.SubmittedByUserId = null;
		timesheetWeekToReopen.ReviewedAtUtc = null;
		timesheetWeekToReopen.ReviewedByUserId = null;
		timesheetWeekToReopen.RejectionReason = null;
		timesheetWeekToReopen.UpdatedAtUtc = nowUtc;

		var statusEventToCreate = new TimesheetWeekStatusEvent
		{
			TimesheetWeekId = timesheetWeekToReopen.Id,
			FromStatus = previousStatus,
			ToStatus = TimesheetWeekStatus.Draft,
			ChangedAtUtc = nowUtc,
			ChangedByUserId = command.ReopenedByUserId,
			Note = command.Note
		};

		context.TimesheetWeekStatusEvents.Add(statusEventToCreate);

		await context.SaveChangesAsync(cancellationToken);
	}
}
