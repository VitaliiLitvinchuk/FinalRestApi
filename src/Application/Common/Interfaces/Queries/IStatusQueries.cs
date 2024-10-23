using Domain.Statuses;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IStatusQueries
{
    public Task<Option<Status>> GetByIdAsync(StatusId id, CancellationToken cancellation);
    public Task<IEnumerable<Status>> GetAllAsync(CancellationToken cancellation);
}
