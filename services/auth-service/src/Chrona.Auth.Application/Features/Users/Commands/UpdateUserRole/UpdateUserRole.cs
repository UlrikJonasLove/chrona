using Chrona.Auth.Application.Interfaces;
using Chrona.Auth.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Users.Commands.UpdateUserRoles;

public record UpdateUserRolesCommand(
	long OrganisationId,
	long UserId,
	IReadOnlyList<string> Roles)
	: IRequest;

public class UpdateUserRolesCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<UpdateUserRolesCommand>
{
	public async Task Handle(UpdateUserRolesCommand command, CancellationToken cancellationToken)
	{
		if (command.OrganisationId <= 0)
			throw new InvalidOperationException("OrganisationId is required.");

		if (command.UserId <= 0)
			throw new InvalidOperationException("UserId is required.");

		var roleNames = command.Roles
			.Where(role => !string.IsNullOrWhiteSpace(role))
			.Select(role => role.Trim())
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		if (roleNames.Count == 0)
			throw new InvalidOperationException("At least one role is required.");

		var userExists = await context.Users
			.AsNoTracking()
			.AnyAsync(
				userEntity => userEntity.Id == command.UserId && userEntity.OrganisationId == command.OrganisationId,
				cancellationToken);

		if (!userExists)
			throw new InvalidOperationException("User not found.");

		var roles = await context.Roles
			.AsNoTracking()
			.Where(roleEntity => roleNames.Contains(roleEntity.Name))
			.ToListAsync(cancellationToken);

		if (roles.Count != roleNames.Count)
			throw new InvalidOperationException("One or more roles do not exist.");

		var existingUserRoles = await context.UserRoles
			.AsTracking()
			.Where(userRoleEntity => userRoleEntity.UserId == command.UserId)
			.ToListAsync(cancellationToken);

		if (existingUserRoles.Count > 0)
			context.UserRoles.RemoveRange(existingUserRoles);

		var nowUtc = DateTime.UtcNow;

		foreach (var role in roles)
		{
			context.UserRoles.Add(new UserRole
			{
				UserId = command.UserId,
				RoleId = role.Id,
				CreatedAtUtc = nowUtc
			});
		}

		await context.SaveChangesAsync(cancellationToken);
	}
}
