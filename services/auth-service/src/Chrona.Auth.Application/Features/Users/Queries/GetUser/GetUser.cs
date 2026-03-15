using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(
	long OrganisationId,
	long UserId)
	: IRequest<UserDetailsDTO>;

public record UserDetailsDTO(
	long Id,
	long OrganisationId,
	string Email,
	string Firstname,
	string Lastname,
	bool IsActive,
	IReadOnlyList<string> Roles);

public class GetUserByIdQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetUserByIdQuery, UserDetailsDTO>
{
	public async Task<UserDetailsDTO> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
	{
		if (query.OrganisationId <= 0)
			throw new InvalidOperationException("OrganisationId is required.");

		if (query.UserId <= 0)
			throw new InvalidOperationException("UserId is required.");

		var user = await context.Users
			.AsNoTracking()
			.Where(userEntity => userEntity.OrganisationId == query.OrganisationId && userEntity.Id == query.UserId)
			.Select(userEntity => new
			{
				userEntity.Id,
				userEntity.OrganisationId,
				userEntity.Email,
				userEntity.Firstname,
				userEntity.Lastname,
				userEntity.IsActive
			})
			.SingleOrDefaultAsync(cancellationToken)
			?? throw new InvalidOperationException("User not found.");

		var roles = await (
			from userRole in context.UserRoles.AsNoTracking()
			join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
			where userRole.UserId == user.Id
			select role.Name
		).Distinct().OrderBy(x => x).ToListAsync(cancellationToken);

		return new UserDetailsDTO(
			user.Id,
			user.OrganisationId,
			user.Email,
			user.Firstname,
			user.Lastname,
			user.IsActive,
			roles);
	}
}
