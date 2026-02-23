using Chrona.Time.Application.Features.TimesheetWeeks.DTOs;
using Chrona.Time.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimesheetWeeks.GetSummary;

public record GetTimesheetWeekSummaryQuery(
	long UserId,
	int IsoYear,
	int IsoWeek)
	: IRequest<TimesheetWeekSummaryDTO>;

public class GetTimesheetWeekSummaryQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetTimesheetWeekSummaryQuery, TimesheetWeekSummaryDTO>
{
	public async Task<TimesheetWeekSummaryDTO> Handle(GetTimesheetWeekSummaryQuery query, CancellationToken cancellationToken)
	{
		var timesheetWeekEntity = await context.TimesheetWeeks
			.AsNoTracking()
			.SingleOrDefaultAsync(timesheetWeek =>
				timesheetWeek.UserId == query.UserId &&
				timesheetWeek.IsoYear == query.IsoYear &&
				timesheetWeek.IsoWeek == query.IsoWeek,
				cancellationToken) ?? throw new InvalidOperationException("Timesheet week was not found.");

		var daySummaries = await context.TimeEntries
			.AsNoTracking()
			.Where(timeEntry => timeEntry.TimesheetWeekId == timesheetWeekEntity.Id)
			.GroupBy(timeEntry => timeEntry.WorkDate)
			.Select(timeEntriesByDate => new TimesheetWeekDaySummaryDTO(
				timeEntriesByDate.Key,
				timeEntriesByDate.Sum(timeEntry => timeEntry.Minutes)))
			.OrderBy(daySummary => daySummary.WorkDate)
			.ToListAsync(cancellationToken);

		return TimesheetWeekSummaryDTO.Create(timesheetWeekEntity, daySummaries);
	}
}
