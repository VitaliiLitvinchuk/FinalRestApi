using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.UserGroupRoles;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserGroupRoleRepository(ApplicationDbContext context) : IUserGroupRoleRepository, IUserGroupRoleQueries
{
    private DbSet<UserGroupRole> UserGroupRoles { get; } = context.UserGroupRoles;
    public async Task<UserGroupRole> Create(UserGroupRole userGroupRole, CancellationToken cancellation)
    {
        await UserGroupRoles.AddAsync(userGroupRole, cancellation);

        await context.SaveChangesAsync(cancellation);

        return userGroupRole;
    }

    public async Task<UserGroupRole> Delete(UserGroupRole userGroupRole, CancellationToken cancellation)
    {
        UserGroupRoles.Remove(userGroupRole);

        await context.SaveChangesAsync(cancellation);

        return userGroupRole;
    }

    public async Task<IEnumerable<UserGroupRole>> GetAllAsync(CancellationToken cancellation)
    {
        return await UserGroupRoles
            .Include(x => x.UserGroups)
            .AsNoTracking()
            .ToListAsync(cancellation);
    }

    public async Task<Option<UserGroupRole>> GetByIdAsync(UserGroupRoleId id, CancellationToken cancellation)
    {
        var userGroupRole = await UserGroupRoles
            .Include(x => x.UserGroups)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        return userGroupRole is null ? Option.None<UserGroupRole>() : Option.Some(userGroupRole);
    }

    public async Task<UserGroupRole> Update(UserGroupRole userGroupRole, CancellationToken cancellation)
    {
        UserGroupRoles.Update(userGroupRole);

        await context.SaveChangesAsync(cancellation);

        return userGroupRole;
    }
}
