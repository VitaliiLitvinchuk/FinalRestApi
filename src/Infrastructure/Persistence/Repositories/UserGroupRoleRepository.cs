using System.Linq.Expressions;
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

        return (await GetByIdAsync(userGroupRole.Id, cancellation)).Match(x => x, () => throw new Exception("Could not create user group role"));
    }

    public async Task<UserGroupRole> Delete(UserGroupRole userGroupRole, CancellationToken cancellation)
    {
        UserGroupRoles.Remove(userGroupRole);

        await context.SaveChangesAsync(cancellation);

        return userGroupRole;
    }

    public async Task<IEnumerable<UserGroupRole>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<UserGroupRole, bool>>? filter = null,
        Func<IQueryable<UserGroupRole>, IOrderedQueryable<UserGroupRole>>? orderBy = null,
        params Expression<Func<UserGroupRole, object?>>[] includes)
    {
        IQueryable<UserGroupRole> query = UserGroupRoles.AsQueryable()
                                                        .AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        foreach (var include in includes)
            query = query.Include(include);

        if (orderBy != null)
            return await orderBy(query).ToListAsync(cancellation);

        return await query.ToListAsync(cancellation);
    }

    public async Task<Option<UserGroupRole>> GetByIdAsync(UserGroupRoleId id, CancellationToken cancellation)
    {
        var userGroupRole = await UserGroupRoles
            .Include(x => x.UserGroups)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        return userGroupRole is null ? Option.None<UserGroupRole>() : Option.Some(userGroupRole);
    }

    public async Task<Option<UserGroupRole>> GetByNameAsync(string name, CancellationToken cancellation)
    {
        var userGroupRole = await UserGroupRoles
            .Include(x => x.UserGroups)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellation);

        return userGroupRole is null ? Option.None<UserGroupRole>() : Option.Some(userGroupRole);
    }

    public async Task<UserGroupRole> Update(UserGroupRole userGroupRole, CancellationToken cancellation)
    {
        UserGroupRoles.Update(userGroupRole);

        await context.SaveChangesAsync(cancellation);

        return (await GetByIdAsync(userGroupRole.Id, cancellation)).Match(x => x, () => throw new Exception("Could not update user group role"));
    }
}
