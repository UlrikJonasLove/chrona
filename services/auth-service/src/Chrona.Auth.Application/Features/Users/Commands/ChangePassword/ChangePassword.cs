using System.Security.Claims;
using Chrona.Auth.Application.Common.Helpers;
using Chrona.Auth.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Auth.Application.Features.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
	string CurrentPassword,
	string NewPassword)
	: IRequest;

public class ChangePasswordCommandHandler(
	IApplicationDbContext context,
	IHttpContextAccessor httpContextAccessor)
	: IRequestHandler<ChangePasswordCommand>
{
	public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(command.CurrentPassword))
			throw new InvalidOperationException("CurrentPassword is required.");

		if (string.IsNullOrWhiteSpace(command.NewPassword))
			throw new InvalidOperationException("NewPassword is required.");

		if (command.NewPassword.Length < 10)
			throw new InvalidOperationException("NewPassword must be at least 10 characters long.");

		var httpContext = httpContextAccessor.HttpContext
			?? throw new UnauthorizedAccessException("Unauthorized.");

		var userId = GetRequiredLongClaim(httpContext.User, "sub");
		var organisationId = GetRequiredLongClaim(httpContext.User, "organisation_id");

		var user = await context.Users
			.AsTracking()
			.SingleOrDefaultAsync(
				userEntity => userEntity.Id == userId && userEntity.OrganisationId == organisationId,
				cancellationToken)
			?? throw new UnauthorizedAccessException("Unauthorized.");

		if (!PasswordHelper.VerifyPassword(command.CurrentPassword, user.Password))
			throw new UnauthorizedAccessException("Invalid credentials.");

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
