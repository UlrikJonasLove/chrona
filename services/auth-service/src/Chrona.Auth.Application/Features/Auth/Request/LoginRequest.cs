namespace Chrona.Auth.Application.Features.Auth.Login;

public sealed record LoginRequest(
	string Email,
	string Password);
