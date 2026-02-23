using Chrona.Time.Application.Features.TimesheetWeeks.DTOs;
using Chrona.Time.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimesheetWeeks.GetByWeek;

public record GetTimesheetWeekQuery(
	long UserId,
	int IsoYear,
	int IsoWeek)
	: IRequest<TimesheetWeekDTO>;

public class GetTimesheetWeekQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetTimesheetWeekQuery, TimesheetWeekDTO>
{
	public async Task<TimesheetWeekDTO> Handle(GetTimesheetWeekQuery query, CancellationToken cancellationToken)
	{
		var timesheetWeekEntity = await context.TimesheetWeeks
			.AsNoTracking()
			.SingleOrDefaultAsync(timesheetWeek =>
				timesheetWeek.UserId == query.UserId &&
				timesheetWeek.IsoYear == query.IsoYear &&
				timesheetWeek.IsoWeek == query.IsoWeek,
				cancellationToken);

		return timesheetWeekEntity is null
			? throw new InvalidOperationException("Timesheet week was not found.")
			: TimesheetWeekDTO.FromEntity(timesheetWeekEntity);
	}
}
