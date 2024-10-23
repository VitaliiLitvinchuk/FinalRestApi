using Domain.Groups;

namespace Application.Common.Interfaces.Repositories;

public interface IGroupRepository
{
    public Task<Group> Create(Group group, CancellationToken cancellation);
    public Task<Group> Update(Group group, CancellationToken cancellation);
    public Task<Group> Delete(Group group, CancellationToken cancellation);
}
