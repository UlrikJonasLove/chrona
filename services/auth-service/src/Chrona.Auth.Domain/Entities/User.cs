namespace Chrona.Auth.Domain.Entities;

public class User
{
	public long Id { get; set; }
	public required string Firstname { get; set; }
	public required string Lastname { get; set; }

	public long OrganisationId { get; set; }
	public Organisation Organisation { get; set; } = null!;

	public string Email { get; set; } = null!;
	public string Password { get; set; } = null!;

	public bool IsActive { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime? LastLoginAtUtc { get; set; }

	public ICollection<UserRole> UserRoles { get; set; } = [];
	public ICollection<UserRefreshSession> UserRefreshSessions { get; set; } = [];
}
