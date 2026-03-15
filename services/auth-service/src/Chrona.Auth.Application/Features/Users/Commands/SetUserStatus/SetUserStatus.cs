using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Users.Commands.SetUserStatus;

public record SetUserActiveStatusCommand(
	long OrganisationId,
	long UserId,
	bool IsActive)
	: IRequest;

public class SetUserActiveStatusCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<SetUserActiveStatusCommand>
{
	public async Task Handle(SetUserActiveStatusCommand command, CancellationToken cancellationToken)
	{
		if (command.OrganisationId <= 0)
			throw new InvalidOperationException("OrganisationId is required.");

		if (command.UserId <= 0)
			throw new InvalidOperationException("UserId is required.");

		var user = await context.Users
			.AsTracking()
			.SingleOrDefaultAsync(
				userEntity => userEntity.Id == command.UserId && userEntity.OrganisationId == command.OrganisationId,
				cancellationToken)
			?? throw new InvalidOperationException("User not found.");

		if (user.IsActive == command.IsActive)
			return;

		user.IsActive = command.IsActive;

		await context.SaveChangesAsync(cancellationToken);
	}
}
