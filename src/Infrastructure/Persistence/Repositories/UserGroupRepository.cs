using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Groups;
using Domain.UserGroupRoles;
using Domain.Users;
using Domain.UsersGroups;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserGroupRepository(ApplicationDbContext context) : IUserGroupRepository, IUserGroupQueries
{
    private DbSet<UserGroup> UserGroups { get; } = context.UserGroups;
    public async Task<UserGroup> Create(UserGroup userGroup, CancellationToken cancellation)
    {
        await UserGroups.AddAsync(userGroup, cancellation);

        await context.SaveChangesAsync(cancellation);

        return userGroup;
    }

    public async Task<UserGroup> Delete(UserGroup userGroup, CancellationToken cancellation)
    {
        UserGroups.Remove(userGroup);

        await context.SaveChangesAsync(cancellation);

        return userGroup;
    }

    public async Task<IEnumerable<UserGroup>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<UserGroup, bool>>? filter = null,
        Func<IQueryable<UserGroup>, IOrderedQueryable<UserGroup>>? orderBy = null,
        params Expression<Func<UserGroup, object?>>[] includes)
    {
        IQueryable<UserGroup> query = UserGroups.AsQueryable()
                                            .AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        foreach (var include in includes)
            query = query.Include(include);

        if (orderBy != null)
            return await orderBy(query).ToListAsync(cancellation);

        return await query.ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserGroup>> GetByGroupIdAndUserGroupRoleIdAsync(GroupId groupId, UserGroupRoleId userGroupRoleId, CancellationToken cancellation)
    {
        return await UserGroups
            .Include(x => x.User)
            .Include(x => x.Group)
            .Include(x => x.UserGroupRole)
            .AsNoTracking()
            .Where(x => x.GroupId == groupId && x.UserGroupRoleId == userGroupRoleId)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserGroup>> GetByGroupIdAsync(GroupId groupId, CancellationToken cancellation)
    {
        return await UserGroups
            .Include(x => x.User)
            .Include(x => x.Group)
            .Include(x => x.UserGroupRole)
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .ToListAsync(cancellation);
    }

    public async Task<Option<UserGroup>> GetByUserIdAndGroupIdAsync(UserId userId, GroupId groupId, CancellationToken cancellation)
    {
        var userGroup = await UserGroups
            .Include(x => x.User)
            .Include(x => x.Group)
            .Include(x => x.UserGroupRole)
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.GroupId == groupId)
            .FirstOrDefaultAsync(cancellation);

        return userGroup is null ? Option.None<UserGroup>() : Option.Some(userGroup);
    }

    public async Task<IEnumerable<UserGroup>> GetByUserIdAndUserGroupRoleIdAsync(UserId userId, UserGroupRoleId userGroupRoleId, CancellationToken cancellation)
    {
        return await UserGroups
            .Include(x => x.User)
            .Include(x => x.Group)
            .Include(x => x.UserGroupRole)
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.UserGroupRoleId == userGroupRoleId)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserGroup>> GetByUserIdAsync(UserId userId, CancellationToken cancellation)
    {
        return await UserGroups
            .Include(x => x.User)
            .Include(x => x.Group)
            .Include(x => x.UserGroupRole)
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellation);
    }

    public async Task<UserGroup> Update(UserGroup userGroup, CancellationToken cancellation)
    {
        UserGroups.Update(userGroup);

        await context.SaveChangesAsync(cancellation);

        return userGroup;
    }
}
