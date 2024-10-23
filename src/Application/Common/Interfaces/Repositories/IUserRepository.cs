using Domain.Users;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<User> Create(User user, CancellationToken cancellation);
    public Task<User> Update(User user, CancellationToken cancellation);
    public Task<User> Delete(User user, CancellationToken cancellation);
}
