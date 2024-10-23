using Domain.UserGroupRoles;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserGroupRoleQueries
{
    public Task<Option<UserGroupRole>> GetByIdAsync(UserGroupRoleId id, CancellationToken cancellation);
    public Task<IEnumerable<UserGroupRole>> GetAllAsync(CancellationToken cancellation);
}
