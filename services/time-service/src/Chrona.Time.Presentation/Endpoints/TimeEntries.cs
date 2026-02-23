using Chrona.Time.Application.Features.TimeEntries.Commands.Create;
using Chrona.Time.Application.Features.TimeEntries.Delete;
using Chrona.Time.Application.Features.TimeEntries.DTOs;
using Chrona.Time.Application.Features.TimeEntries.GetByDateRange;
using Chrona.Time.Application.Features.TimeEntries.GetById;
using Chrona.Time.Application.Features.TimeEntries.GetByWeek;
using Chrona.Time.Application.Features.TimeEntries.Update;
using Chrona.Time.Presentation.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Chrona.Time.Presentation.Endpoints;

public class TimeEntries : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Create time entry",
				Description = "Creates a new time entry. Supports manual entries and timer-based entries (StartNow/StartedAtUtc/StoppedAtUtc).",
				Responses = new OpenApiResponses
				{
					["201"] = new OpenApiResponse
					{
						Description = "Created",
						Content = new Dictionary<string, OpenApiMediaType>
						{
							["application/json"] = new OpenApiMediaType()
						}
					},
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["409"] = new OpenApiResponse { Description = "Conflict" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapPost(CreateAsync);

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Update time entry",
				Description = "Updates an existing time entry. Ongoing entries (StartedAtUtc set and StoppedAtUtc null) cannot be updated.",
				Responses = new OpenApiResponses
				{
					["204"] = new OpenApiResponse { Description = "No Content" },
					["400"] = new OpenApiResponse { Description = "Bad Request" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["409"] = new OpenApiResponse { Description = "Conflict" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapPut(UpdateAsync, "{timeEntryId:long}");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Delete time entry",
				Description = "Deletes an existing time entry.",
				Responses = new OpenApiResponses
				{
					["204"] = new OpenApiResponse { Description = "No Content" },
					["401"] = new OpenApiResponse { Description = "Unauthorized" },
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["409"] = new OpenApiResponse { Description = "Conflict" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapDelete(DeleteAsync, "{timeEntryId:long}");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get time entry by id",
				Description = "Gets a single time entry by its id.",
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
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapGet(GetByIdAsync, "{timeEntryId:long}");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get time entries by week",
				Description = "Gets time entries for a given ISO year and ISO week for a user.",
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
			.MapGet(GetByWeekAsync, "by-week");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get time entries by date range",
				Description = "Gets time entries between FromDate and ToDate (inclusive) for a user.",
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
			.MapGet(GetByDateRangeAsync, "by-date-range");
	}

	public async Task<Created<long>> CreateAsync(ISender sender, [FromBody] CreateTimeEntryCommand command) =>
		TypedResults.Created(string.Empty, await sender.Send(command));

	public async Task<NoContent> UpdateAsync(
		ISender sender,
		[FromRoute] long timeEntryId,
		[FromBody] UpdateTimeEntryCommand command)
	{
		if (command.TimeEntryId != timeEntryId)
			throw new BadHttpRequestException("Route timeEntryId must match command TimeEntryId.");

		await sender.Send(command);
		return TypedResults.NoContent();
	}

	public async Task<NoContent> DeleteAsync(ISender sender, [FromRoute] long timeEntryId)
	{
		await sender.Send(new DeleteTimeEntryCommand(timeEntryId));
		return TypedResults.NoContent();
	}

	public async Task<Ok<TimeEntryDTO>> GetByIdAsync(ISender sender, [FromRoute] long timeEntryId) =>
		TypedResults.Ok(await sender.Send(new GetTimeEntryByIdQuery(timeEntryId)));

	public async Task<Ok<IReadOnlyList<TimeEntryDTO>>> GetByWeekAsync(
		ISender sender,
		[AsParameters] GetTimeEntriesByWeekQuery query) =>
		TypedResults.Ok(await sender.Send(query));

	public async Task<Ok<IReadOnlyList<TimeEntryDTO>>> GetByDateRangeAsync(
		ISender sender,
		[AsParameters] GetTimeEntriesByDateRangeQuery query) =>
		TypedResults.Ok(await sender.Send(query));
}
