using Chrona.Time.Domain.Entities;

namespace Chrona.Time.Application.Features.TimesheetWeeks.DTOs;

public record TimesheetWeekSummaryDTO(
	long Id,
	long UserId,
	int IsoYear,
	int IsoWeek,
	TimesheetWeekStatus Status,
	int TotalMinutes,
	IReadOnlyList<TimesheetWeekDaySummaryDTO> Days)
{
	public static TimesheetWeekSummaryDTO Create(
		TimesheetWeek timesheetWeek,
		IReadOnlyList<TimesheetWeekDaySummaryDTO> days) =>
		new(
			timesheetWeek.Id,
			timesheetWeek.UserId,
			timesheetWeek.IsoYear,
			timesheetWeek.IsoWeek,
			timesheetWeek.Status,
			days.Sum(day => day.TotalMinutes),
			days);
}

public record TimesheetWeekDaySummaryDTO(
	DateOnly WorkDate,
	int TotalMinutes);
