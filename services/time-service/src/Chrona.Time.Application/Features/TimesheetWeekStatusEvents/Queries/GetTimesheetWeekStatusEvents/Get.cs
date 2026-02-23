using Chrona.Time.Application.Features.TimesheetWeekStatusEvents.DTOs;
using Chrona.Time.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimesheetWeekStatusEvents.GetByTimesheetWeek;

public record GetTimesheetWeekStatusEventsQuery(long TimesheetWeekId)
	: IRequest<IReadOnlyList<TimesheetWeekStatusEventDTO>>;

public class GetTimesheetWeekStatusEventsQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetTimesheetWeekStatusEventsQuery, IReadOnlyList<TimesheetWeekStatusEventDTO>>
{
	public async Task<IReadOnlyList<TimesheetWeekStatusEventDTO>> Handle(GetTimesheetWeekStatusEventsQuery query, CancellationToken cancellationToken)
	{
		var statusEventEntities = await context.TimesheetWeekStatusEvents
			.AsNoTracking()
			.Where(statusEvent => statusEvent.TimesheetWeekId == query.TimesheetWeekId)
			.OrderByDescending(statusEvent => statusEvent.ChangedAtUtc)
			.ToListAsync(cancellationToken);

		var statusEvents = statusEventEntities
			.Select(TimesheetWeekStatusEventDTO.FromEntity)
			.ToList();

		return statusEvents;
	}
}
