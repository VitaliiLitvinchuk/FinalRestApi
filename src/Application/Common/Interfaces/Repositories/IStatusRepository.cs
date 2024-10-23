using Domain.Statuses;

namespace Application.Common.Interfaces.Repositories;

public interface IStatusRepository
{
    public Task<Status> Create(Status status, CancellationToken cancellation);
    public Task<Status> Update(Status status, CancellationToken cancellation);
    public Task<Status> Delete(Status status, CancellationToken cancellation);
}
