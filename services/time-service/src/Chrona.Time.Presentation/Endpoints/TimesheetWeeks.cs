using Chrona.Time.Application.Features.TimesheetWeeks.Approve;
using Chrona.Time.Application.Features.TimesheetWeeks.DTOs;
using Chrona.Time.Application.Features.TimesheetWeeks.GetById;
using Chrona.Time.Application.Features.TimesheetWeeks.GetByWeek;
using Chrona.Time.Application.Features.TimesheetWeeks.GetSummary;
using Chrona.Time.Application.Features.TimesheetWeeks.Reject;
using Chrona.Time.Application.Features.TimesheetWeeks.Reopen;
using Chrona.Time.Application.Features.TimesheetWeeks.Submit;
using Chrona.Time.Presentation.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Chrona.Time.Presentation.Endpoints;

public class TimesheetWeeks : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get timesheet week by id",
				Description = "Gets a single timesheet week by its id.",
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
			.MapGet(GetByIdAsync, "{timesheetWeekId:long}");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get timesheet week by week",
				Description = "Gets a timesheet week for a user by ISO year and ISO week.",
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
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapGet(GetByWeekAsync, "by-week");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get timesheet week summary",
				Description = "Gets a timesheet week summary including day totals for a user by ISO year and ISO week.",
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
					["404"] = new OpenApiResponse { Description = "Not Found" },
					["500"] = new OpenApiResponse { Description = "Internal Server Error" }
				},
			})
			.MapGet(GetSummaryAsync, "summary");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Submit timesheet week",
				Description = "Submits a draft timesheet week. The week must have at least one time entry and no ongoing entries.",
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
			.MapPost(SubmitAsync, "{timesheetWeekId:long}/submit");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Approve timesheet week",
				Description = "Approves a submitted timesheet week.",
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
			.MapPost(ApproveAsync, "{timesheetWeekId:long}/approve");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Reject timesheet week",
				Description = "Rejects a submitted timesheet week and stores a rejection reason.",
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
			.MapPost(RejectAsync, "{timesheetWeekId:long}/reject");

		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Reopen timesheet week",
				Description = "Reopens a rejected timesheet week back to draft. Clears submitted/reviewed fields. Optional note can be stored in status event.",
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
			.MapPost(ReopenAsync, "{timesheetWeekId:long}/reopen");
	}

	public async Task<Ok<TimesheetWeekDTO>> GetByIdAsync(ISender sender, [FromRoute] long timesheetWeekId) =>
		TypedResults.Ok(await sender.Send(new GetTimesheetWeekByIdQuery(timesheetWeekId)));

	public async Task<Ok<TimesheetWeekDTO>> GetByWeekAsync(
		ISender sender,
		[AsParameters] GetTimesheetWeekQuery query) =>
		TypedResults.Ok(await sender.Send(query));

	public async Task<Ok<TimesheetWeekSummaryDTO>> GetSummaryAsync(
		ISender sender,
		[AsParameters] GetTimesheetWeekSummaryQuery query) =>
		TypedResults.Ok(await sender.Send(query));

	public async Task<NoContent> SubmitAsync(
		ISender sender,
		[FromRoute] long timesheetWeekId,
		[FromBody] SubmitTimesheetWeekCommand command)
	{
		if (command.TimesheetWeekId != timesheetWeekId)
			throw new BadHttpRequestException("Route timesheetWeekId must match command TimesheetWeekId.");

		await sender.Send(command);
		return TypedResults.NoContent();
	}

	public async Task<NoContent> ApproveAsync(
		ISender sender,
		[FromRoute] long timesheetWeekId,
		[FromBody] ApproveTimesheetWeekCommand command)
	{
		if (command.TimesheetWeekId != timesheetWeekId)
			throw new BadHttpRequestException("Route timesheetWeekId must match command TimesheetWeekId.");

		await sender.Send(command);
		return TypedResults.NoContent();
	}

	public async Task<NoContent> RejectAsync(
		ISender sender,
		[FromRoute] long timesheetWeekId,
		[FromBody] RejectTimesheetWeekCommand command)
	{
		if (command.TimesheetWeekId != timesheetWeekId)
			throw new BadHttpRequestException("Route timesheetWeekId must match command TimesheetWeekId.");

		await sender.Send(command);
		return TypedResults.NoContent();
	}

	public async Task<NoContent> ReopenAsync(
		ISender sender,
		[FromRoute] long timesheetWeekId,
		[FromBody] ReopenTimesheetWeekCommand command)
	{
		if (command.TimesheetWeekId != timesheetWeekId)
			throw new BadHttpRequestException("Route timesheetWeekId must match command TimesheetWeekId.");

		await sender.Send(command);
		return TypedResults.NoContent();
	}
}
