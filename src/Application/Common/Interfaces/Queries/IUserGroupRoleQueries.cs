using System.Linq.Expressions;
using Domain.UserGroupRoles;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserGroupRoleQueries
{
    public Task<Option<UserGroupRole>> GetByIdAsync(UserGroupRoleId id, CancellationToken cancellation);
    public Task<Option<UserGroupRole>> GetByNameAsync(string name, CancellationToken cancellation);
    public Task<IEnumerable<UserGroupRole>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<UserGroupRole, bool>>? filter = null,
        Func<IQueryable<UserGroupRole>, IOrderedQueryable<UserGroupRole>>? orderBy = null,
        params Expression<Func<UserGroupRole, object?>>[] includes);
}
