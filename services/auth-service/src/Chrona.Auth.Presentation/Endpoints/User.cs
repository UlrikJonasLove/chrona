using Chrona.Auth.Application.Features.Sessions.Commands.RevokeUserSession;
using Chrona.Auth.Application.Features.Sessions.Queries.GetUserSessions;
using Chrona.Auth.Application.Features.Users.Commands.ChangePassword;
using Chrona.Auth.Application.Features.Users.Commands.CreateUser;
using Chrona.Auth.Application.Features.Users.Commands.ResetUserPassword;
using Chrona.Auth.Application.Features.Users.Commands.SetUserStatus;
using Chrona.Auth.Application.Features.Users.Commands.UpdateUserRoles;
using Chrona.Auth.Application.Features.Users.Queries.GetUserById;
using Chrona.Auth.Application.Features.Users.Queries.GetUsers;
using Chrona.Auth.Presentation.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Chrona.Auth.Presentation.Endpoints;

public class Users : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Create user",
				Description = "Creates a new user within an organisation and assigns roles.",
				Responses = new OpenApiResponses
				{
					["201"] = new OpenApiResponse
					{
						Description = "Created",
						Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
					},
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["409"] = new OpenApiResponse { Description = "Conflict" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapPost(CreateUserAsync);

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get users",
				Description = "Gets users for a given organisation.",
				Responses = new OpenApiResponses
				{
					["200"] = new OpenApiResponse
					{
						Description = "OK",
						Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
					},
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapGet(GetUsersAsync);

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get user by id",
				Description = "Gets a single user by id within an organisation.",
				Responses = new OpenApiResponses
				{
					["200"] = new OpenApiResponse
					{
						Description = "OK",
						Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
					},
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapGet(GetUserByIdAsync, "{userId:long}");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Update user roles",
				Description = "Replaces all roles for a user within an organisation.",
				Responses = new OpenApiResponses
				{
					["204"] = new OpenApiResponse { Description = "No Content" },
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapPut(UpdateUserRolesAsync, "{userId:long}/roles");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Set user active status",
				Description = "Activates or deactivates a user within an organisation.",
				Responses = new OpenApiResponses
				{
					["204"] = new OpenApiResponse { Description = "No Content" },
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapPut(SetUserActiveStatusAsync, "{userId:long}/active");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Reset user password",
				Description = "Admin resets a user's password and revokes all active sessions.",
				Responses = new OpenApiResponses
				{
					["204"] = new OpenApiResponse { Description = "No Content" },
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapPost(ResetUserPasswordAsync, "{userId:long}/reset-password");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get user sessions",
				Description = "Gets refresh sessions for a user within an organisation.",
				Responses = new OpenApiResponses
				{
					["200"] = new OpenApiResponse
					{
						Description = "OK",
						Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = new OpenApiMediaType() }
					},
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapGet(GetUserSessionsAsync, "{userId:long}/sessions");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Revoke user session",
				Description = "Revokes a specific refresh session for a user within an organisation.",
				Responses = new OpenApiResponses
				{
					["204"] = new OpenApiResponse { Description = "No Content" },
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapPost(RevokeUserSessionAsync, "{userId:long}/sessions/{userRefreshSessionId:long}/revoke");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Change password",
				Description = "Changes password for the current user and revokes all active sessions.",
				Responses = new OpenApiResponses
				{
					["204"] = new OpenApiResponse { Description = "No Content" },
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				}
			})
			.MapPost(ChangePasswordAsync, "change-password");
	}

	public async Task<Created<long>> CreateUserAsync(ISender sender, [FromBody] CreateUserCommand command) =>
		TypedResults.Created(string.Empty, await sender.Send(command));

	public async Task<Ok<IReadOnlyList<UserListItemDTO>>> GetUsersAsync(ISender sender, [FromQuery] long organisationId) =>
		TypedResults.Ok(await sender.Send(new GetUsersQuery(organisationId)));

	public async Task<Ok<UserDetailsDTO>> GetUserByIdAsync(
		ISender sender,
		[FromRoute] long userId,
		[FromQuery] long organisationId) =>
		TypedResults.Ok(await sender.Send(new GetUserByIdQuery(organisationId, userId)));

	public async Task<NoContent> UpdateUserRolesAsync(
		ISender sender,
		[FromRoute] long userId,
		[FromBody] UpdateUserRolesRequest request)
	{
		await sender.Send(new UpdateUserRolesCommand(request.OrganisationId, userId, request.Roles));
		return TypedResults.NoContent();
	}

	public async Task<NoContent> SetUserActiveStatusAsync(
		ISender sender,
		[FromRoute] long userId,
		[FromBody] SetUserActiveStatusRequest request)
	{
		await sender.Send(new SetUserActiveStatusCommand(request.OrganisationId, userId, request.IsActive));
		return TypedResults.NoContent();
	}

	public async Task<NoContent> ResetUserPasswordAsync(
		ISender sender,
		[FromRoute] long userId,
		[FromBody] ResetUserPasswordRequest request)
	{
		await sender.Send(new ResetUserPasswordCommand(request.OrganisationId, userId, request.NewPassword));
		return TypedResults.NoContent();
	}

	public async Task<Ok<IReadOnlyList<UserSessionDTO>>> GetUserSessionsAsync(
		ISender sender,
		[FromRoute] long userId,
		[FromQuery] long organisationId) =>
		TypedResults.Ok(await sender.Send(new GetUserSessionsQuery(organisationId, userId)));

	public async Task<NoContent> RevokeUserSessionAsync(
		ISender sender,
		[FromRoute] long userId,
		[FromRoute] long userRefreshSessionId,
		[FromQuery] long organisationId)
	{
		await sender.Send(new RevokeUserSessionCommand(organisationId, userId, userRefreshSessionId));
		return TypedResults.NoContent();
	}

	public async Task<NoContent> ChangePasswordAsync(ISender sender, [FromBody] ChangePasswordCommand command)
	{
		await sender.Send(command);
		return TypedResults.NoContent();
	}
}

public sealed record UpdateUserRolesRequest(
	long OrganisationId,
	IReadOnlyList<string> Roles);

public sealed record SetUserActiveStatusRequest(
	long OrganisationId,
	bool IsActive);

public sealed record ResetUserPasswordRequest(
	long OrganisationId,
	string NewPassword);
