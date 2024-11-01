using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.UserRoles;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserRoleRepository(ApplicationDbContext context) : IUserRoleRepository, IUserRoleQueries
{
    private DbSet<UserRole> UserRoles { get; } = context.UserRoles;
    public async Task<UserRole> Create(UserRole userRole, CancellationToken cancellation)
    {
        await UserRoles.AddAsync(userRole, cancellation);

        await context.SaveChangesAsync(cancellation);

        return userRole;
    }

    public async Task<UserRole> Delete(UserRole userRole, CancellationToken cancellation)
    {
        UserRoles.Remove(userRole);

        await context.SaveChangesAsync(cancellation);

        return userRole;
    }

    public async Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<UserRole, bool>>? filter = null,
        Func<IQueryable<UserRole>, IOrderedQueryable<UserRole>>? orderBy = null,
        params Expression<Func<UserRole, object?>>[] includes)
    {
        IQueryable<UserRole> query = UserRoles;

        if (filter != null)
            query = query.Where(filter);

        foreach (var include in includes)
            query = query.Include(include);

        if (orderBy != null)
            return await orderBy(query).ToListAsync(cancellation);

        return await query.ToListAsync(cancellation);
    }

    public async Task<Option<UserRole>> GetByIdAsync(UserRoleId id, CancellationToken cancellation)
    {
        var userRole = await UserRoles
            .Include(x => x.Users)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        return userRole is null ? Option.None<UserRole>() : Option.Some(userRole);
    }

    public async Task<Option<UserRole>> GetByNameAsync(string name, CancellationToken cancellation)
    {
        var userRole = await UserRoles
            .Include(x => x.Users)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellation);


        return userRole is null ? Option.None<UserRole>() : Option.Some(userRole);
    }

    public async Task<UserRole> Update(UserRole userRole, CancellationToken cancellation)
    {
        UserRoles.Update(userRole);

        await context.SaveChangesAsync(cancellation);

        return userRole;
    }
}
