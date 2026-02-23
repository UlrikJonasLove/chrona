using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimesheetWeeks.Reject;

public record RejectTimesheetWeekCommand(
	long TimesheetWeekId,
	long ReviewedByUserId,
	string RejectionReason)
	: IRequest;

public class RejectTimesheetWeekCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<RejectTimesheetWeekCommand>
{
	public async Task Handle(RejectTimesheetWeekCommand command, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(command.RejectionReason))
			throw new InvalidOperationException("Rejection reason cannot be empty.");

		var timesheetWeekToReject = await context.TimesheetWeeks
			.AsTracking()
			.FirstAsync(timesheetWeek => timesheetWeek.Id == command.TimesheetWeekId, cancellationToken);

		if (timesheetWeekToReject.Status is TimesheetWeekStatus.Rejected)
			return;

		if (timesheetWeekToReject.Status is not TimesheetWeekStatus.Submitted)
			throw new InvalidOperationException("Only submitted timesheet weeks can be rejected.");

		var nowUtc = DateTime.UtcNow;
		var previousStatus = timesheetWeekToReject.Status;

		timesheetWeekToReject.Status = TimesheetWeekStatus.Rejected;
		timesheetWeekToReject.ReviewedAtUtc = nowUtc;
		timesheetWeekToReject.ReviewedByUserId = command.ReviewedByUserId;
		timesheetWeekToReject.RejectionReason = command.RejectionReason;
		timesheetWeekToReject.UpdatedAtUtc = nowUtc;

		var statusEventToCreate = new TimesheetWeekStatusEvent
		{
			TimesheetWeekId = timesheetWeekToReject.Id,
			FromStatus = previousStatus,
			ToStatus = TimesheetWeekStatus.Rejected,
			ChangedAtUtc = nowUtc,
			ChangedByUserId = command.ReviewedByUserId,
			Note = command.RejectionReason
		};

		context.TimesheetWeekStatusEvents.Add(statusEventToCreate);

		await context.SaveChangesAsync(cancellationToken);
	}
}
