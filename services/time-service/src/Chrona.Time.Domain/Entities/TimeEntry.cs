namespace Chrona.Time.Domain.Entities;

public class TimeEntry
{
	public long Id { get; set; }

	public long TimesheetWeekId { get; set; }
	public TimesheetWeek TimesheetWeek { get; set; } = null!;

	public long UserId { get; set; }
	public long ProjectId { get; set; }

	public DateOnly WorkDate { get; set; }
	public int Minutes { get; set; }

	public DateTime? StartedAtUtc { get; set; }
	public DateTime? StoppedAtUtc { get; set; }

	public string? Comment { get; set; }

	public DateTime CreatedAtUtc { get; set; }
	public DateTime UpdatedAtUtc { get; set; }

	public byte[] RowVersion { get; set; } = [];
}
