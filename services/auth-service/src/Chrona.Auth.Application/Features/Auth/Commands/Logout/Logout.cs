using Chrona.Auth.Application.Common.Helpers;
using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(
	string RefreshToken)
	: IRequest;

public class LogoutCommandHandler(
	IApplicationDbContext context,
	RefreshTokenHelper refreshTokenHelper)
	: IRequestHandler<LogoutCommand>
{
	public async Task Handle(LogoutCommand command, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(command.RefreshToken))
			throw new UnauthorizedAccessException("Refresh token is missing.");

		var refreshTokenHash = refreshTokenHelper.HashRefreshToken(command.RefreshToken);

		var session = await context.UserRefreshSessions
			.AsTracking()
			.SingleOrDefaultAsync(
				sessionEntity => sessionEntity.RefreshTokenHash == refreshTokenHash,
				cancellationToken);

		if (session is null)
			return;

		if (session.RevokedAtUtc is not null)
			return;

		session.RevokedAtUtc = DateTime.UtcNow;

		await context.SaveChangesAsync(cancellationToken);
	}
}
