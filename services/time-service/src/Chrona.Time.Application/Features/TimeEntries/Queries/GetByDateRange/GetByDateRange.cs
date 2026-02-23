using Chrona.Time.Application.Features.TimeEntries.DTOs;
using Chrona.Time.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimeEntries.GetByDateRange;

public record GetTimeEntriesByDateRangeQuery(
	long UserId,
	DateOnly FromDate,
	DateOnly ToDate)
	: IRequest<IReadOnlyList<TimeEntryDTO>>;

public class GetTimeEntriesByDateRangeQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetTimeEntriesByDateRangeQuery, IReadOnlyList<TimeEntryDTO>>
{
	public async Task<IReadOnlyList<TimeEntryDTO>> Handle(GetTimeEntriesByDateRangeQuery query, CancellationToken cancellationToken)
	{
		if (query.ToDate < query.FromDate)
			throw new InvalidOperationException("ToDate cannot be earlier than FromDate.");

		var timeEntryEntities = await context.TimeEntries
			.AsNoTracking()
			.Where(timeEntry =>
				timeEntry.UserId == query.UserId &&
				timeEntry.WorkDate >= query.FromDate &&
				timeEntry.WorkDate <= query.ToDate)
			.OrderBy(timeEntry => timeEntry.WorkDate)
			.ThenBy(timeEntry => timeEntry.ProjectId)
			.ToListAsync(cancellationToken);

		var timeEntries = timeEntryEntities
			.Select(TimeEntryDTO.FromEntity)
			.ToList();

		return timeEntries;
	}
}
