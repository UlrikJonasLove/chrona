using Chrona.Time.Application.Features.TimeEntries.DTOs;
using Chrona.Time.Application.Interfaces;
using Chrona.Time.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimeEntries.GetByWeek;

public record GetTimeEntriesByWeekQuery(
	long UserId,
	int IsoYear,
	int IsoWeek)
	: IRequest<IReadOnlyList<TimeEntryDTO>>;

public class GetTimeEntriesByWeekQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetTimeEntriesByWeekQuery, IReadOnlyList<TimeEntryDTO>>
{
	public async Task<IReadOnlyList<TimeEntryDTO>> Handle(GetTimeEntriesByWeekQuery query, CancellationToken cancellationToken)
	{
		var timesheetWeekId = await context.TimesheetWeeks
			.AsNoTracking()
			.Where(timesheetWeek =>
				timesheetWeek.UserId == query.UserId &&
				timesheetWeek.IsoYear == query.IsoYear &&
				timesheetWeek.IsoWeek == query.IsoWeek)
			.Select(timesheetWeek => (long?)timesheetWeek.Id)
			.SingleOrDefaultAsync(cancellationToken);

		if (timesheetWeekId is null)
			return [];

		var timeEntryEntities = await context.TimeEntries
		.AsNoTracking()
		.Where(timeEntry =>
			timeEntry.UserId == query.UserId &&
			timeEntry.TimesheetWeekId == timesheetWeekId.Value)
		.OrderBy(timeEntry => timeEntry.WorkDate)
		.ThenBy(timeEntry => timeEntry.ProjectId)
		.ToListAsync(cancellationToken);

		var timeEntries = timeEntryEntities
				.Select(TimeEntryDTO.FromEntity)
				.ToList();

		return timeEntries;
	}
}
