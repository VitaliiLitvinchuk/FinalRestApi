using Domain.Groups;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IGroupQueries
{
    public Task<Option<Group>> GetByIdAsync(GroupId id, CancellationToken cancellation);
    public Task<IEnumerable<Group>> GetAllAsync(CancellationToken cancellation);
}
