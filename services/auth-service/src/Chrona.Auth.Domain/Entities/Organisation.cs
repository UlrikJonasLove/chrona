namespace Chrona.Auth.Domain.Entities;

public class Organisation
{
	public long Id { get; set; }

	public string Name { get; set; } = null!;
	public string Slug { get; set; } = null!;

	public bool IsActive { get; set; }
	public DateTime CreatedAtUtc { get; set; }

	public ICollection<User> Users { get; set; } = [];
}
