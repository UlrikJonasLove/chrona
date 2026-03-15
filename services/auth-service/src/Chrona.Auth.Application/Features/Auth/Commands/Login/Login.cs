using Chrona.Auth.Application.Common.Helpers;
using Chrona.Auth.Application.Interfaces;
using Chrona.Auth.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Auth.Commands.Login;

public record LoginCommand(
	string Email,
	string Password)
	: IRequest<LoginResult>;

public record LoginResult(
	string AccessToken,
	string RefreshToken);

public class LoginCommandHandler(
	IApplicationDbContext context,
	ITokenService tokenService,
	RefreshTokenHelper refreshTokenHelper)
	: IRequestHandler<LoginCommand, LoginResult>
{
	public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
	{
		var email = command.Email.Trim().ToLowerInvariant();

		if (string.IsNullOrWhiteSpace(email))
			throw new InvalidOperationException("Email is required.");

		if (string.IsNullOrWhiteSpace(command.Password))
			throw new InvalidOperationException("Password is required.");

		var user = await context.Users
			.AsNoTracking()
			.SingleOrDefaultAsync(
				userEntity => userEntity.Email == email,
				cancellationToken)
			?? throw new UnauthorizedAccessException("Invalid credentials.");

		if (!user.IsActive)
			throw new UnauthorizedAccessException("User is inactive.");

		if (!PasswordHelper.VerifyPassword(command.Password, user.Password))
			throw new UnauthorizedAccessException("Invalid credentials.");

		var roles = await (
			from userRole in context.UserRoles.AsNoTracking()
			join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
			where userRole.UserId == user.Id
			select role.Name
		).Distinct().ToListAsync(cancellationToken);

		var refreshToken = refreshTokenHelper.GenerateRefreshToken();
		var refreshTokenHash = refreshTokenHelper.HashRefreshToken(refreshToken);

		var nowUtc = DateTime.UtcNow;

		var refreshSessionToCreate = new UserRefreshSession
		{
			UserId = user.Id,
			OrganisationId = user.OrganisationId,
			RefreshTokenHash = refreshTokenHash,
			CreatedAtUtc = nowUtc,
			ExpiresAtUtc = nowUtc.AddDays(30)
		};

		context.UserRefreshSessions.Add(refreshSessionToCreate);
		await context.SaveChangesAsync(cancellationToken);

		var accessToken = tokenService.GenerateJwt(user, refreshSessionToCreate.Id, roles);

		return new LoginResult(accessToken, refreshToken);
	}
}
