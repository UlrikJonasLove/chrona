namespace Chrona.Time.Domain.Entities;

public class TimesheetWeek
{
	public long Id { get; set; }

	public long UserId { get; set; }

	public int IsoYear { get; set; }
	public int IsoWeek { get; set; }

	public TimesheetWeekStatus Status { get; set; }

	public DateTime? SubmittedAtUtc { get; set; }
	public long? SubmittedByUserId { get; set; }

	public DateTime? ReviewedAtUtc { get; set; }
	public long? ReviewedByUserId { get; set; }

	public string? RejectionReason { get; set; }

	public DateTime CreatedAtUtc { get; set; }
	public DateTime UpdatedAtUtc { get; set; }

	public byte[] RowVersion { get; set; } = [];

	public ICollection<TimeEntry> TimeEntries { get; set; } = [];
	public ICollection<TimesheetWeekStatusEvent> StatusEvents { get; set; } = [];
}
