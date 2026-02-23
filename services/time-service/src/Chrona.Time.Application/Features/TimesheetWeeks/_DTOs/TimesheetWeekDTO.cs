using Chrona.Time.Domain.Entities;

namespace Chrona.Time.Application.Features.TimesheetWeeks.DTOs;

public record TimesheetWeekDTO(
	long Id,
	long UserId,
	int IsoYear,
	int IsoWeek,
	TimesheetWeekStatus Status,
	DateTime? SubmittedAtUtc,
	long? SubmittedByUserId,
	DateTime? ReviewedAtUtc,
	long? ReviewedByUserId,
	string? RejectionReason,
	DateTime CreatedAtUtc,
	DateTime UpdatedAtUtc)
{
	public static TimesheetWeekDTO FromEntity(TimesheetWeek timesheetWeek) =>
		new(
			timesheetWeek.Id,
			timesheetWeek.UserId,
			timesheetWeek.IsoYear,
			timesheetWeek.IsoWeek,
			timesheetWeek.Status,
			timesheetWeek.SubmittedAtUtc,
			timesheetWeek.SubmittedByUserId,
			timesheetWeek.ReviewedAtUtc,
			timesheetWeek.ReviewedByUserId,
			timesheetWeek.RejectionReason,
			timesheetWeek.CreatedAtUtc,
			timesheetWeek.UpdatedAtUtc);
}
