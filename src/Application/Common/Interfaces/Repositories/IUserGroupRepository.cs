using Domain.UsersGroups;

namespace Application.Common.Interfaces.Repositories;

public interface IUserGroupRepository
{
    public Task<UserGroup> Create(UserGroup userGroup, CancellationToken cancellation);
    public Task<UserGroup> Update(UserGroup userGroup, CancellationToken cancellation);
    public Task<UserGroup> Delete(UserGroup userGroup, CancellationToken cancellation);
}
