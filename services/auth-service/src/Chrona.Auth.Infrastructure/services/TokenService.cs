using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chrona.Auth.Application.Interfaces;
using Chrona.Auth.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Chrona.Auth.Infrastructure.Services;

public class TokenService(IConfiguration config) : ITokenService
{
	private readonly SymmetricSecurityKey _key = new(
		Encoding.UTF8.GetBytes(config["JwtKey"]!));

	public string GenerateJwt(User user, long refreshSessionId, IEnumerable<string> roles)
	{
		var now = DateTime.UtcNow;

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.Email, user.Email),
			new("organisation_id", user.OrganisationId.ToString()),
			new("sid", refreshSessionId.ToString()),

			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
		};

		foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
			claims.Add(new Claim(ClaimTypes.Role, role));

		var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);
		var token = new JwtSecurityToken(
			claims: claims,
			expires: now.AddMinutes(10),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
	{
		var parameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = _key,
			ValidateLifetime = false,
			NameClaimType = JwtRegisteredClaimNames.Email,
			RoleClaimType = ClaimTypes.Role
		};

		var handler = new JwtSecurityTokenHandler();
		try
		{
			var principal = handler.ValidateToken(token, parameters, out var securityToken);
			if (securityToken is not JwtSecurityToken jwt || jwt.Header.Alg is not SecurityAlgorithms.HmacSha512)
				return null;

			return principal;
		}
		catch { return null; }
	}
}
