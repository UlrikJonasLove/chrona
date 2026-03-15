namespace Chrona.Auth.Domain.Entities;

public class UserRefreshSession
{
	public long Id { get; set; }

	public long UserId { get; set; }
	public User User { get; set; } = null!;

	public long OrganisationId { get; set; }
	public Organisation Organisation { get; set; } = null!;

	public byte[] RefreshTokenHash { get; set; } = null!;

	public DateTime CreatedAtUtc { get; set; }
	public DateTime ExpiresAtUtc { get; set; }

	public DateTime? RevokedAtUtc { get; set; }

	public long? ReplacedByUserRefreshSessionId { get; set; }
	public UserRefreshSession? ReplacedByUserRefreshSession { get; set; }

	public string? UserAgent { get; set; }
	public string? IpAddress { get; set; }
}
