using Domain.UserRoles;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRoleRepository
{
    public Task<UserRole> Create(UserRole userRole, CancellationToken cancellation);
    public Task<UserRole> Update(UserRole userRole, CancellationToken cancellation);
    public Task<UserRole> Delete(UserRole userRole, CancellationToken cancellation);
}
