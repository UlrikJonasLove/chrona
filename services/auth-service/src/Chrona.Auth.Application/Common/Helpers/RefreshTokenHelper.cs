using System.Security.Cryptography;
using System.Text;

namespace Chrona.Auth.Application.Common.Helpers;

public sealed class RefreshTokenHelper
{
	private readonly byte[] _pepper;

	public RefreshTokenHelper(string pepper)
	{
		if (string.IsNullOrWhiteSpace(pepper))
			throw new InvalidOperationException("RefreshTokenPepper is not configured.");

		_pepper = Encoding.UTF8.GetBytes(pepper);
	}

	public string GenerateRefreshToken() =>
		Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

	public byte[] HashRefreshToken(string refreshToken)
	{
		if (string.IsNullOrWhiteSpace(refreshToken))
			throw new InvalidOperationException("Refresh token is required.");

		using var hmac = new HMACSHA512(_pepper);
		return hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
	}
}
