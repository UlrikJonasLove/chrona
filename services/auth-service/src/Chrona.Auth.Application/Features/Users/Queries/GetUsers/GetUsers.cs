using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(
	long OrganisationId)
	: IRequest<IReadOnlyList<UserListItemDTO>>;

public record UserListItemDTO(
	long Id,
	string Email,
	string Firstname,
	string Lastname,
	bool IsActive,
	IReadOnlyList<string> Roles);

public class GetUsersQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetUsersQuery, IReadOnlyList<UserListItemDTO>>
{
	public async Task<IReadOnlyList<UserListItemDTO>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
	{
		if (query.OrganisationId <= 0)
			throw new InvalidOperationException("OrganisationId is required.");

		var users = await context.Users
			.AsNoTracking()
			.Where(userEntity => userEntity.OrganisationId == query.OrganisationId)
			.OrderBy(userEntity => userEntity.Lastname)
			.ThenBy(userEntity => userEntity.Firstname)
			.Select(userEntity => new
			{
				userEntity.Id,
				userEntity.Email,
				userEntity.Firstname,
				userEntity.Lastname,
				userEntity.IsActive
			})
			.ToListAsync(cancellationToken);

		if (users.Count == 0)
			return [];

		var userIds = users.Select(u => u.Id).ToList();

		var rolesByUserId = await (
			from userRole in context.UserRoles.AsNoTracking()
			join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
			where userIds.Contains(userRole.UserId)
			select new { userRole.UserId, role.Name }
		).ToListAsync(cancellationToken);

		var rolesLookup = rolesByUserId
			.GroupBy(x => x.UserId)
			.ToDictionary(
				group => group.Key,
				group => (IReadOnlyList<string>)group
					.Select(x => x.Name)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.OrderBy(x => x)
					.ToList());

		return users
			.Select(u => new UserListItemDTO(
				u.Id,
				u.Email,
				u.Firstname,
				u.Lastname,
				u.IsActive,
				rolesLookup.TryGetValue(u.Id, out var roles) ? roles : []))
			.ToList();
	}
}
