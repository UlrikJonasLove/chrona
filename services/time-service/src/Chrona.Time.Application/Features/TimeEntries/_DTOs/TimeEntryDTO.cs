using Chrona.Time.Domain.Entities;

namespace Chrona.Time.Application.Features.TimeEntries.DTOs;

public record TimeEntryDTO(
	long Id,
	long TimesheetWeekId,
	long UserId,
	long ProjectId,
	DateOnly WorkDate,
	int Minutes,
	DateTime? StartedAtUtc,
	DateTime? StoppedAtUtc,
	string? Comment)
{
	public static TimeEntryDTO FromEntity(TimeEntry timeEntry) =>
		new(
			timeEntry.Id,
			timeEntry.TimesheetWeekId,
			timeEntry.UserId,
			timeEntry.ProjectId,
			timeEntry.WorkDate,
			timeEntry.Minutes,
			timeEntry.StartedAtUtc,
			timeEntry.StoppedAtUtc,
			timeEntry.Comment);
}
