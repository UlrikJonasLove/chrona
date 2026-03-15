using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chrona.Auth.Application.Features.Users.DTOs;
using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<CurrentUserDTO>;

public class GetCurrentUserQueryHandler(
	IApplicationDbContext context,
	IHttpContextAccessor httpContextAccessor)
	: IRequestHandler<GetCurrentUserQuery, CurrentUserDTO>
{
	public async Task<CurrentUserDTO> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
	{
		var httpContext = httpContextAccessor.HttpContext
			?? throw new UnauthorizedAccessException("Unauthorized.");

		var userId = GetRequiredLongClaim(httpContext.User, JwtRegisteredClaimNames.Sub);
		var organisationId = GetRequiredLongClaim(httpContext.User, "organisation_id");

		var user = await context.Users
			.AsNoTracking()
			.SingleOrDefaultAsync(
				userEntity => userEntity.Id == userId && userEntity.OrganisationId == organisationId,
				cancellationToken)
			?? throw new UnauthorizedAccessException("Unauthorized.");

		var roles = await (
			from userRole in context.UserRoles.AsNoTracking()
			join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
			where userRole.UserId == user.Id
			select role.Name
		).Distinct().ToListAsync(cancellationToken);

		return new CurrentUserDTO(
			user.Id,
			user.OrganisationId,
			user.Email,
			user.Firstname,
			user.Lastname,
			roles);
	}

	private static long GetRequiredLongClaim(ClaimsPrincipal principal, string claimType)
	{
		var value = principal.FindFirst(claimType)?.Value;
		if (string.IsNullOrWhiteSpace(value))
			throw new UnauthorizedAccessException("Unauthorized.");

		if (!long.TryParse(value, out var parsed))
			throw new UnauthorizedAccessException("Unauthorized.");

		return parsed;
	}
}
