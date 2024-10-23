using System;
using Domain.UserGroupRoles;

namespace Application.Common.Interfaces.Repositories;

public interface IUserGroupRoleRepository
{
    public Task<UserGroupRole> Create(UserGroupRole userGroupRole, CancellationToken cancellation);
    public Task<UserGroupRole> Update(UserGroupRole userGroupRole, CancellationToken cancellation);
    public Task<UserGroupRole> Delete(UserGroupRole userGroupRole, CancellationToken cancellation);
}
