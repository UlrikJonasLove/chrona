using Chrona.Time.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Interfaces;

public interface IApplicationDbContext
{
	public DbSet<TimeEntry> TimeEntries { get; }
	public DbSet<TimesheetWeek> TimesheetWeeks { get; }
	public DbSet<TimesheetWeekStatusEvent> TimesheetWeekStatusEvents { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
