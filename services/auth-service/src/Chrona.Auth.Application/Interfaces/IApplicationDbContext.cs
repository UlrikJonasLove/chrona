using Chrona.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Interfaces;

public interface IApplicationDbContext
{
	public DbSet<Organisation> Organisations { get; }
	public DbSet<Role> Roles { get; }
	public DbSet<User> Users { get; }
	public DbSet<UserRefreshSession> UserRefreshSessions { get; }
	public DbSet<UserRole> UserRoles { get; }


	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
