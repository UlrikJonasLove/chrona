namespace Chrona.Time.Domain.Entities;

public class TimesheetWeekStatusEvent
{
	public long Id { get; set; }

	public long TimesheetWeekId { get; set; }
	public TimesheetWeek TimesheetWeek { get; set; } = null!;

	public TimesheetWeekStatus? FromStatus { get; set; }
	public TimesheetWeekStatus ToStatus { get; set; }

	public DateTime ChangedAtUtc { get; set; }
	public long ChangedByUserId { get; set; }

	public string? Note { get; set; }
}
