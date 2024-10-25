using Domain.UserRoles;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserRoleQueries
{
    public Task<Option<UserRole>> GetByIdAsync(UserRoleId id, CancellationToken cancellation);
    public Task<Option<UserRole>> GetByNameAsync(string name, CancellationToken cancellation);
    public Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellation);
}
