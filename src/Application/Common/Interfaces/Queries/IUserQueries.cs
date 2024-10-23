using Domain.UserRoles;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserQueries
{
    public Task<Option<User>> GetByIdAsync(UserId id, CancellationToken cancellation);
    public Task<Option<User>> GetByEmailAsync(string email, CancellationToken cancellation);
    public Task<IEnumerable<User>> GetByRoleIdAsync(UserRoleId roleId, CancellationToken cancellation);
    public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellation);
}
