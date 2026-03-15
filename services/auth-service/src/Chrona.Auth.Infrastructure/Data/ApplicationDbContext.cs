using Chrona.Auth.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Chrona.Auth.Domain.Entities;

namespace Chrona.Auth.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
	public DbSet<Organisation> Organisations => Set<Organisation>();
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<User> Users => Set<User>();
	public DbSet<UserRefreshSession> UserRefreshSessions => Set<UserRefreshSession>();
	public DbSet<UserRole> UserRoles => Set<UserRole>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}
