using Chrona.Auth.Application.Common.Helpers;
using Chrona.Auth.Application.Interfaces;
using Chrona.Auth.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
	long OrganisationId,
	string Email,
	string Password,
	string Firstname,
	string Lastname,
	IReadOnlyList<string> Roles)
	: IRequest<long>;

public class CreateUserCommandHandler(
	IApplicationDbContext context)
	: IRequestHandler<CreateUserCommand, long>
{
	public async Task<long> Handle(CreateUserCommand command, CancellationToken cancellationToken)
	{
		if (command.OrganisationId <= 0)
			throw new InvalidOperationException("OrganisationId is required.");

		var email = command.Email.Trim().ToLowerInvariant();

		if (string.IsNullOrWhiteSpace(email))
			throw new InvalidOperationException("Email is required.");

		if (string.IsNullOrWhiteSpace(command.Password))
			throw new InvalidOperationException("Password is required.");

		if (string.IsNullOrWhiteSpace(command.Firstname))
			throw new InvalidOperationException("Firstname is required.");

		if (string.IsNullOrWhiteSpace(command.Lastname))
			throw new InvalidOperationException("Lastname is required.");

		var organisationExists = await context.Organisations
			.AsNoTracking()
			.AnyAsync(org => org.Id == command.OrganisationId, cancellationToken);

		if (!organisationExists)
			throw new InvalidOperationException("Organisation not found.");

		var emailTaken = await context.Users
			.AsNoTracking()
			.AnyAsync(
				userEntity => userEntity.OrganisationId == command.OrganisationId && userEntity.Email.ToLower() == email,
				cancellationToken);

		if (emailTaken)
			throw new InvalidOperationException("Email is already in use.");

		var roleNames = command.Roles
			.Where(role => !string.IsNullOrWhiteSpace(role))
			.Select(role => role.Trim())
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		var roles = await context.Roles
			.AsNoTracking()
			.Where(roleEntity => roleNames.Contains(roleEntity.Name))
			.ToListAsync(cancellationToken);

		if (roles.Count != roleNames.Count)
			throw new InvalidOperationException("One or more roles do not exist.");

		var nowUtc = DateTime.UtcNow;

		var userToCreate = new User
		{
			OrganisationId = command.OrganisationId,
			Email = email,
			Password = PasswordHelper.HashPassword(command.Password),
			Firstname = command.Firstname.Trim(),
			Lastname = command.Lastname.Trim(),
			IsActive = true,
			CreatedAtUtc = nowUtc
		};

		context.Users.Add(userToCreate);

		foreach (var role in roles)
		{
			context.UserRoles.Add(new UserRole
			{
				User = userToCreate,
				RoleId = role.Id,
				CreatedAtUtc = nowUtc
			});
		}

		await context.SaveChangesAsync(cancellationToken);

		return userToCreate.Id;
	}
}
