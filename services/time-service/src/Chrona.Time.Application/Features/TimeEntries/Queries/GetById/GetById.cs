using Chrona.Time.Application.Features.TimeEntries.DTOs;
using Chrona.Time.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrona.Time.Application.Features.TimeEntries.GetById;

public record GetTimeEntryByIdQuery(long TimeEntryId)
	: IRequest<TimeEntryDTO>;

public class GetTimeEntryByIdQueryHandler(
	IApplicationDbContext context)
	: IRequestHandler<GetTimeEntryByIdQuery, TimeEntryDTO>
{
	public async Task<TimeEntryDTO> Handle(GetTimeEntryByIdQuery query, CancellationToken cancellationToken)
	{
		var timeEntryEntity = await context.TimeEntries
			.AsNoTracking()
			.SingleOrDefaultAsync(timeEntry => timeEntry.Id == query.TimeEntryId, cancellationToken);

		return timeEntryEntity is null
			? throw new InvalidOperationException("Time entry was not found.")
			: TimeEntryDTO.FromEntity(timeEntryEntity);
	}
}
