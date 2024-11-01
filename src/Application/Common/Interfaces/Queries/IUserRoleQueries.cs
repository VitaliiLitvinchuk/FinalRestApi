using System.Linq.Expressions;
using Domain.UserRoles;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserRoleQueries
{
    public Task<Option<UserRole>> GetByIdAsync(UserRoleId id, CancellationToken cancellation);
    public Task<Option<UserRole>> GetByNameAsync(string name, CancellationToken cancellation);
    public Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<UserRole, bool>>? filter = null,
        Func<IQueryable<UserRole>, IOrderedQueryable<UserRole>>? orderBy = null,
        params Expression<Func<UserRole, object?>>[] includes);
}
