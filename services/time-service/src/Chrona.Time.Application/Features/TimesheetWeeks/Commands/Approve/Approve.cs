using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimesheetWeeks.Approve;

public record ApproveTimesheetWeekCommand(
	long TimesheetWeekId,
	long ReviewedByUserId)
	: IRequest;

public class ApproveTimesheetWeekCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<ApproveTimesheetWeekCommand>
{
	public async Task Handle(ApproveTimesheetWeekCommand command, CancellationToken cancellationToken)
	{
		var timesheetWeekToApprove = await context.TimesheetWeeks
			.AsTracking()
			.FirstAsync(timesheetWeek => timesheetWeek.Id == command.TimesheetWeekId, cancellationToken);

		if (timesheetWeekToApprove.Status is TimesheetWeekStatus.Approved)
			return;

		if (timesheetWeekToApprove.Status is not TimesheetWeekStatus.Submitted)
			throw new InvalidOperationException("Only submitted timesheet weeks can be approved.");

		var previousStatus = timesheetWeekToApprove.Status;

		timesheetWeekToApprove.Status = TimesheetWeekStatus.Approved;
		timesheetWeekToApprove.ReviewedAtUtc = DateTime.UtcNow;
		timesheetWeekToApprove.ReviewedByUserId = command.ReviewedByUserId;
		timesheetWeekToApprove.RejectionReason = null;
		timesheetWeekToApprove.UpdatedAtUtc = DateTime.UtcNow;

		var statusEventToCreate = new TimesheetWeekStatusEvent
		{
			TimesheetWeekId = timesheetWeekToApprove.Id,
			FromStatus = previousStatus,
			ToStatus = TimesheetWeekStatus.Approved,
			ChangedAtUtc = DateTime.UtcNow,
			ChangedByUserId = command.ReviewedByUserId,
			Note = null
		};

		context.TimesheetWeekStatusEvents.Add(statusEventToCreate);

		await context.SaveChangesAsync(cancellationToken);
	}
}
