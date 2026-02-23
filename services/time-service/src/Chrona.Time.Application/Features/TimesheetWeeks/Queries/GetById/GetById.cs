using Chrona.Time.Application.Features.TimesheetWeeks.DTOs;
using Chrona.Time.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimesheetWeeks.GetById;

public record GetTimesheetWeekByIdQuery(long TimesheetWeekId)
	: IRequest<TimesheetWeekDTO>;

public class GetTimesheetWeekByIdQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetTimesheetWeekByIdQuery, TimesheetWeekDTO>
{
	public async Task<TimesheetWeekDTO> Handle(GetTimesheetWeekByIdQuery query, CancellationToken cancellationToken)
	{
		var timesheetWeekEntity = await context.TimesheetWeeks
			.AsNoTracking()
			.SingleOrDefaultAsync(timesheetWeek => timesheetWeek.Id == query.TimesheetWeekId, cancellationToken);

		return timesheetWeekEntity is null
			? throw new InvalidOperationException("Timesheet week was not found.")
			: TimesheetWeekDTO.FromEntity(timesheetWeekEntity);
	}
}
