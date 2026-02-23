using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Chrona.Time.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
	public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
	public DbSet<TimesheetWeek> TimesheetWeeks => Set<TimesheetWeek>();
	public DbSet<TimesheetWeekStatusEvent> TimesheetWeekStatusEvents => Set<TimesheetWeekStatusEvent>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}
