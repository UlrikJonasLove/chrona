using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Sessions.Commands.RevokeUserSession;

public record RevokeUserSessionCommand(
	long OrganisationId,
	long UserId,
	long UserRefreshSessionId)
	: IRequest;

public class RevokeUserSessionCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<RevokeUserSessionCommand>
{
	public async Task Handle(RevokeUserSessionCommand command, CancellationToken cancellationToken)
	{
		if (command.OrganisationId <= 0)
			throw new InvalidOperationException("OrganisationId is required.");

		if (command.UserId <= 0)
			throw new InvalidOperationException("UserId is required.");

		if (command.UserRefreshSessionId <= 0)
			throw new InvalidOperationException("UserRefreshSessionId is required.");

		var session = await context.UserRefreshSessions
			.AsTracking()
			.SingleOrDefaultAsync(
				sessionEntity =>
					sessionEntity.Id == command.UserRefreshSessionId &&
					sessionEntity.UserId == command.UserId &&
					sessionEntity.OrganisationId == command.OrganisationId,
				cancellationToken)
			?? throw new InvalidOperationException("Session not found.");

		if (session.RevokedAtUtc is not null)
			return;

		session.RevokedAtUtc = DateTime.UtcNow;

		await context.SaveChangesAsync(cancellationToken);
	}
}
