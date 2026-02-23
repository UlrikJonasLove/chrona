using Chrona.Time.Domain.Entities;

namespace Chrona.Time.Application.Features.TimesheetWeekStatusEvents.DTOs;
public record TimesheetWeekStatusEventDTO(
	long Id,
	long TimesheetWeekId,
	TimesheetWeekStatus? FromStatus,
	TimesheetWeekStatus ToStatus,
	DateTime ChangedAtUtc,
	long ChangedByUserId,
	string? Note)
{
	public static TimesheetWeekStatusEventDTO FromEntity(TimesheetWeekStatusEvent statusEvent) =>
		new(
			statusEvent.Id,
			statusEvent.TimesheetWeekId,
			statusEvent.FromStatus,
			statusEvent.ToStatus,
			statusEvent.ChangedAtUtc,
			statusEvent.ChangedByUserId,
			statusEvent.Note);
}
