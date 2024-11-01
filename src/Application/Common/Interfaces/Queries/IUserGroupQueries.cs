using System.Linq.Expressions;
using Domain.Groups;
using Domain.UserGroupRoles;
using Domain.Users;
using Domain.UsersGroups;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserGroupQueries
{
    public Task<Option<UserGroup>> GetByUserIdAndGroupIdAsync(UserId userId, GroupId groupId, CancellationToken cancellation);
    public Task<IEnumerable<UserGroup>> GetByGroupIdAsync(GroupId groupId, CancellationToken cancellation);
    public Task<IEnumerable<UserGroup>> GetByUserIdAsync(UserId userId, CancellationToken cancellation);
    public Task<IEnumerable<UserGroup>> GetByUserIdAndUserGroupRoleIdAsync(UserId userId, UserGroupRoleId userGroupRoleId, CancellationToken cancellation);
    public Task<IEnumerable<UserGroup>> GetByGroupIdAndUserGroupRoleIdAsync(GroupId groupId, UserGroupRoleId userGroupRoleId, CancellationToken cancellation);
    public Task<IEnumerable<UserGroup>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<UserGroup, bool>>? filter = null,
        Func<IQueryable<UserGroup>, IOrderedQueryable<UserGroup>>? orderBy = null,
        params Expression<Func<UserGroup, object?>>[] includes);
}
