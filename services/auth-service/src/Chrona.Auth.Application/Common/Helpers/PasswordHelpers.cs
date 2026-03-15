using System.Security.Cryptography;
using System.Text;

namespace Chrona.Auth.Application.Common.Helpers;

public static class PasswordHelper
{
	private const char Separator = ';';

	public static string HashPassword(string password)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new InvalidOperationException("Password is required.");

		using var hmac = new HMACSHA512();

		var salt = hmac.Key;
		var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

		var hashBase64 = Convert.ToBase64String(hash);
		var saltBase64 = Convert.ToBase64String(salt);

		return $"{hashBase64}{Separator}{saltBase64}";
	}

	public static bool VerifyPassword(string password, string storedPassword)
	{
		if (string.IsNullOrWhiteSpace(password))
			return false;

		if (string.IsNullOrWhiteSpace(storedPassword))
			return false;

		var separatorIndex = storedPassword.IndexOf(Separator);
		if (separatorIndex <= 0 || separatorIndex >= storedPassword.Length - 1)
			return false;

		var hashBase64 = storedPassword[..separatorIndex];
		var saltBase64 = storedPassword[(separatorIndex + 1)..];

		byte[] expectedHash;
		byte[] salt;

		try
		{
			expectedHash = Convert.FromBase64String(hashBase64);
			salt = Convert.FromBase64String(saltBase64);
		}
		catch
		{
			return false;
		}

		using var hmac = new HMACSHA512(salt);
		var actualHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

		return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
	}
}
