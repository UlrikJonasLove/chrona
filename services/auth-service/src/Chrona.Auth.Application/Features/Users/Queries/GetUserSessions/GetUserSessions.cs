using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Sessions.Queries.GetUserSessions;

public record GetUserSessionsQuery(
	long OrganisationId,
	long UserId)
	: IRequest<IReadOnlyList<UserSessionDTO>>;

public record UserSessionDTO(
	long Id,
	DateTime CreatedAtUtc,
	DateTime ExpiresAtUtc,
	DateTime? RevokedAtUtc,
	string? UserAgent,
	string? IpAddress);

public class GetUserSessionsQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetUserSessionsQuery, IReadOnlyList<UserSessionDTO>>
{
	public async Task<IReadOnlyList<UserSessionDTO>> Handle(GetUserSessionsQuery query, CancellationToken cancellationToken)
	{
		if (query.OrganisationId <= 0)
			throw new InvalidOperationException("OrganisationId is required.");

		if (query.UserId <= 0)
			throw new InvalidOperationException("UserId is required.");

		var userExists = await context.Users
			.AsNoTracking()
			.AnyAsync(
				userEntity => userEntity.Id == query.UserId && userEntity.OrganisationId == query.OrganisationId,
				cancellationToken);

		if (!userExists)
			throw new InvalidOperationException("User not found.");

		return await context.UserRefreshSessions
			.AsNoTracking()
			.Where(sessionEntity =>
				sessionEntity.UserId == query.UserId &&
				sessionEntity.OrganisationId == query.OrganisationId)
			.OrderByDescending(sessionEntity => sessionEntity.CreatedAtUtc)
			.Select(sessionEntity => new UserSessionDTO(
				sessionEntity.Id,
				sessionEntity.CreatedAtUtc,
				sessionEntity.ExpiresAtUtc,
				sessionEntity.RevokedAtUtc,
				sessionEntity.UserAgent,
				sessionEntity.IpAddress))
			.ToListAsync(cancellationToken);
	}
}
