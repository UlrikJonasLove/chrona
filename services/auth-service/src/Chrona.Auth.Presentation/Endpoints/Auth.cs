using Chrona.Auth.Application.Features.Auth.Commands.Login;
using Chrona.Auth.Application.Features.Auth.Commands.Logout;
using Chrona.Auth.Application.Features.Auth.Commands.Refresh;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Chrona.Auth.Presentation.Infrastructure;

namespace Chrona.Auth.Presentation.Endpoints;

public class Auth : EndpointGroupBase
{
	private const string RefreshTokenCookieName = "refresh_token";
	private const int RefreshTokenDays = 30;

	public override void Map(WebApplication app)
	{
		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Login",
				Description = "Logs in a user and sets the refresh_token cookie.",
				Responses = new OpenApiResponses
				{
					["200"] = new OpenApiResponse
					{
						Description = "OK",
						Content = new Dictionary<string, OpenApiMediaType>
						{
							["application/json"] = new OpenApiMediaType()
						}
					},
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapPost(LoginAsync, "login");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Refresh access token",
				Description = "Refreshes the access token using the refresh_token cookie.",
				Responses = new OpenApiResponses
				{
					["200"] = new OpenApiResponse
					{
						Description = "OK",
						Content = new Dictionary<string, OpenApiMediaType>
						{
							["application/json"] = new OpenApiMediaType()
						}
					},
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapPost(RefreshAsync, "refresh");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Logout",
				Description = "Logs out the current session and clears the refresh_token cookie.",
				Responses = new OpenApiResponses
				{
					["204"] = new OpenApiResponse { Description = "No Content" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapPost(LogoutAsync, "logout");
	}

	public async Task<Ok<LoginResponse>> LoginAsync(
		HttpContext httpContext,
		ISender sender,
		[FromBody] LoginRequest request)
	{
		var result = await sender.Send(new LoginCommand(
			request.Email,
			request.Password));

		SetRefreshTokenCookie(httpContext, result.RefreshToken);

		return TypedResults.Ok(new LoginResponse(result.AccessToken));
	}

	public async Task<Ok<RefreshResponse>> RefreshAsync(
		HttpContext httpContext,
		ISender sender)
	{
		if (!httpContext.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken))
			throw new UnauthorizedAccessException("Refresh token is missing.");

		var accessToken = await sender.Send(new RefreshCommand(refreshToken));

		return TypedResults.Ok(new RefreshResponse(accessToken));
	}

	public async Task<NoContent> LogoutAsync(
		HttpContext httpContext,
		ISender sender)
	{
		if (!httpContext.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken))
			throw new UnauthorizedAccessException("Refresh token is missing.");

		await sender.Send(new LogoutCommand(refreshToken));

		ClearRefreshTokenCookie(httpContext);

		return TypedResults.NoContent();
	}

	private static void SetRefreshTokenCookie(HttpContext httpContext, string refreshToken)
	{
		httpContext.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
		{
			HttpOnly = true,
			Secure = false,
			SameSite = SameSiteMode.Lax,
			Path = "/",
			Expires = DateTimeOffset.UtcNow.AddDays(RefreshTokenDays)
		});
	}

	private static void ClearRefreshTokenCookie(HttpContext httpContext) =>
		httpContext.Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
		{
			Path = "/"
		});
}

public sealed record LoginRequest(
	string Email,
	string Password);

public sealed record LoginResponse(
	string AccessToken);

public sealed record RefreshResponse(
	string AccessToken);
