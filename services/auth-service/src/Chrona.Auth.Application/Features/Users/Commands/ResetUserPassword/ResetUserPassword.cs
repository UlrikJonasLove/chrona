using Chrona.Auth.Application.Common.Helpers;
using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Users.Commands.ResetUserPassword;

public record ResetUserPasswordCommand(
	long OrganisationId,
	long UserId,
	string NewPassword)
	: IRequest;

public class ResetUserPasswordCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<ResetUserPasswordCommand>
{
	public async Task Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
	{
		if (command.OrganisationId <= 0)
			throw new InvalidOperationException("OrganisationId is required.");

		if (command.UserId <= 0)
			throw new InvalidOperationException("UserId is required.");

		if (string.IsNullOrWhiteSpace(command.NewPassword))
			throw new InvalidOperationException("NewPassword is required.");

		if (command.NewPassword.Length < 10)
			throw new InvalidOperationException("NewPassword must be at least 10 characters long.");

		var user = await context.Users
			.AsTracking()
			.SingleOrDefaultAsync(
				userEntity => userEntity.Id == command.UserId && userEntity.OrganisationId == command.OrganisationId,
				cancellationToken)
			?? throw new InvalidOperationException("User not found.");

		user.Password = PasswordHelper.HashPassword(command.NewPassword);

		var nowUtc = DateTime.UtcNow;

		var sessions = await context.UserRefreshSessions
			.AsTracking()
			.Where(sessionEntity =>
				sessionEntity.UserId == user.Id &&
				sessionEntity.RevokedAtUtc == null &&
				sessionEntity.ExpiresAtUtc > nowUtc)
			.ToListAsync(cancellationToken);

		foreach (var session in sessions)
			session.RevokedAtUtc = nowUtc;

		await context.SaveChangesAsync(cancellationToken);
	}
}
