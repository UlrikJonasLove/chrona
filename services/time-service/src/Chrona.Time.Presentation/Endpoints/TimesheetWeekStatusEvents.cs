using Chrona.Time.Application.Features.TimesheetWeekStatusEvents.GetByTimesheetWeek;
using Chrona.Time.Presentation.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using Chrona.Time.Application.Features.TimesheetWeekStatusEvents.DTOs;

namespace Chrona.Time.Presentation.Endpoints;

public class TimesheetWeekStatusEvents : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		app.MapGroup(this)
			.WithOpenApi(operation => new OpenApiOperation(operation)
			{
				Summary = "Get timesheet week status events",
				Description = "Gets status events for a given timesheet week id, ordered by ChangedAtUtc descending.",
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
			.MapGet(GetByTimesheetWeekAsync, "by-timesheet-week");
	}

	public async Task<Ok<IReadOnlyList<TimesheetWeekStatusEventDTO>>> GetByTimesheetWeekAsync(
		ISender sender,
		[AsParameters] GetTimesheetWeekStatusEventsQuery query) =>
		TypedResults.Ok(await sender.Send(query));
}
