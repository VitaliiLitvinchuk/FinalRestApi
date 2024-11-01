using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.UserRoles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository, IUserQueries
{
    private DbSet<User> Users { get; } = context.Users;
    public async Task<User> Create(User user, CancellationToken cancellation)
    {
        await Users.AddAsync(user, cancellation);

        await context.SaveChangesAsync(cancellation);

        return user;
    }

    public async Task<User> Delete(User user, CancellationToken cancellation)
    {
        Users.Remove(user);

        await context.SaveChangesAsync(cancellation);

        return user;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<User, bool>>? filter = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        params Expression<Func<User, object?>>[] includes)
    {
        IQueryable<User> query = Users.AsQueryable()
                                      .AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        foreach (var include in includes)
            query = query.Include(include);

        if (orderBy != null)
            return await orderBy(query).ToListAsync(cancellation);

        return await query.ToListAsync(cancellation);
    }

    public async Task<Option<User>> GetByEmailAsync(string email, CancellationToken cancellation)
    {
        var user = await Users
            .Include(x => x.Courses)
            .Include(x => x.UserAssignments)
            .Include(x => x.UserGroups)
            .Include(x => x.UserRole)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellation);

        return user is null ? Option.None<User>() : Option.Some(user);
    }

    public async Task<Option<User>> GetByIdAsync(UserId id, CancellationToken cancellation)
    {
        var user = await Users
            .Include(x => x.Courses)
            .Include(x => x.UserAssignments)
            .Include(x => x.UserGroups)
            .Include(x => x.UserRole)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        return user is null ? Option.None<User>() : Option.Some(user);
    }

    public async Task<IEnumerable<User>> GetByRoleIdAsync(UserRoleId roleId, CancellationToken cancellation)
    {
        return await Users
            .Include(x => x.Courses)
            .Include(x => x.UserAssignments)
            .Include(x => x.UserGroups)
            .Include(x => x.UserRole)
            .AsNoTracking()
            .ToListAsync(cancellation);
    }

    public async Task<User> Update(User user, CancellationToken cancellation)
    {
        Users.Update(user);

        await context.SaveChangesAsync(cancellation);

        return user;
    }
}
