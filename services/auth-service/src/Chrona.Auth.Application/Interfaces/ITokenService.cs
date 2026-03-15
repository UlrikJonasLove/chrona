using System.Security.Claims;

namespace Chrona.Auth.Application.Interfaces;

public interface ITokenService
{
	string GenerateJwt(Domain.Entities.User user, long refreshSessionId, IEnumerable<string> roles);

	ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
