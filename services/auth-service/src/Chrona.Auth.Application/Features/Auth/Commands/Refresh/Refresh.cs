using Chrona.Auth.Application.Common.Helpers;
using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Auth.Commands.Refresh;

public record RefreshCommand(
	string RefreshToken)
	: IRequest<string>;

public class RefreshCommandHandler(
	IApplicationDbContext context,
	ITokenService tokenService,
	RefreshTokenHelper refreshTokenHelper)
	: IRequestHandler<RefreshCommand, string>
{
	public async Task<string> Handle(RefreshCommand command, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(command.RefreshToken))
			throw new UnauthorizedAccessException("Refresh token is missing.");

		var refreshTokenHash = refreshTokenHelper.HashRefreshToken(command.RefreshToken);

		var session = await context.UserRefreshSessions
			.AsNoTracking()
			.SingleOrDefaultAsync(
				sessionEntity => sessionEntity.RefreshTokenHash == refreshTokenHash,
				cancellationToken)
			?? throw new UnauthorizedAccessException("Invalid refresh token.");

		if (session.RevokedAtUtc is not null)
			throw new UnauthorizedAccessException("Refresh session is revoked.");

		var nowUtc = DateTime.UtcNow;
		if (session.ExpiresAtUtc <= nowUtc)
			throw new UnauthorizedAccessException("Refresh token has expired.");

		var user = await context.Users
			.AsNoTracking()
			.SingleOrDefaultAsync(userEntity => userEntity.Id == session.UserId, cancellationToken)
			?? throw new UnauthorizedAccessException("User not found.");

		if (!user.IsActive)
			throw new UnauthorizedAccessException("User is inactive.");

		var roles = await (
			from userRole in context.UserRoles.AsNoTracking()
			join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
			where userRole.UserId == user.Id
			select role.Name
		).Distinct().ToListAsync(cancellationToken);

		return tokenService.GenerateJwt(user, session.Id, roles);
	}
}
