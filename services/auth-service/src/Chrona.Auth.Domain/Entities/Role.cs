namespace Chrona.Auth.Domain.Entities;

public class Role
{
	public long Id { get; set; }

	public string Name { get; set; } = null!;
	public DateTime CreatedAtUtc { get; set; }

	public ICollection<UserRole> UserRoles { get; set; } = [];
}
