using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class GroupRepository(ApplicationDbContext context) : IGroupRepository, IGroupQueries
{
    private DbSet<Group> Groups { get; } = context.Groups;
    public async Task<Group> Create(Group group, CancellationToken cancellation)
    {
        await Groups.AddAsync(group, cancellation);

        await context.SaveChangesAsync(cancellation);

        return group;
    }

    public async Task<Group> Delete(Group group, CancellationToken cancellation)
    {
        Groups.Remove(group);

        await context.SaveChangesAsync(cancellation);

        return group;
    }

    public async Task<IEnumerable<Group>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<Group, bool>>? filter = null,
        Func<IQueryable<Group>, IOrderedQueryable<Group>>? orderBy = null,
        params Expression<Func<Group, object?>>[] includes)
    {
        IQueryable<Group> query = Groups.AsQueryable()
                                        .AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        foreach (var include in includes)
            query = query.Include(include);

        if (orderBy != null)
            return await orderBy(query).ToListAsync(cancellation);

        return await query.ToListAsync(cancellation);
    }

    public async Task<Option<Group>> GetByIdAsync(GroupId id, CancellationToken cancellation)
    {
        var group = await Groups
            .Include(x => x.Courses)
            .Include(x => x.UserGroups)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        return group is null ? Option.None<Group>() : Option.Some(group);
    }

    public async Task<Group> Update(Group group, CancellationToken cancellation)
    {
        Groups.Update(group);

        await context.SaveChangesAsync(cancellation);

        return group;
    }
}
