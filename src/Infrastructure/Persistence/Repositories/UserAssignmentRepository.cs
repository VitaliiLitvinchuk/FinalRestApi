using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Assignments;
using Domain.Statuses;
using Domain.Users;
using Domain.UsersAssignments;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserAssignmentRepository(ApplicationDbContext context) : IUserAssignmentRepository, IUserAssignmentQueries
{
    private DbSet<UserAssignment> UserAssignments { get; } = context.UserAssignments;
    public async Task<UserAssignment> Create(UserAssignment userAssignment, CancellationToken cancellation)
    {
        await UserAssignments.AddAsync(userAssignment, cancellation);

        await context.SaveChangesAsync(cancellation);

        return userAssignment;
    }

    public async Task<UserAssignment> Delete(UserAssignment userAssignment, CancellationToken cancellation)
    {
        UserAssignments.Remove(userAssignment);

        await context.SaveChangesAsync(cancellation);

        return userAssignment;
    }

    public async Task<IEnumerable<UserAssignment>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<UserAssignment, bool>>? filter = null,
        Func<IQueryable<UserAssignment>, IOrderedQueryable<UserAssignment>>? orderBy = null,
        params Expression<Func<UserAssignment, object?>>[] includes)
    {
        IQueryable<UserAssignment> query = UserAssignments.AsQueryable()
                                                          .AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        foreach (var include in includes)
            query = query.Include(include);

        if (orderBy != null)
            return await orderBy(query).ToListAsync(cancellation);

        return await query.ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserAssignment>> GetByAssignmentIdAndStatusIdAsync(AssignmentId assignmentId, StatusId statusId, CancellationToken cancellation)
    {
        return await UserAssignments
            .Include(x => x.Status)
            .Include(x => x.User)
            .Include(x => x.Assignment)
            .AsNoTracking()
            .Where(x => x.AssignmentId == assignmentId && x.StatusId == statusId)
            .ToListAsync(cancellation);
    }

    public async Task<Option<UserAssignment>> GetByAssignmentIdAndUserIdAsync(UserId userId, AssignmentId assignmentId, CancellationToken cancellation)
    {
        var userAssignment = await UserAssignments
            .Include(x => x.Status)
            .Include(x => x.User)
            .Include(x => x.Assignment)
            .AsNoTracking()
            .Where(x => x.AssignmentId == assignmentId && x.UserId == userId)
            .FirstOrDefaultAsync(cancellation);

        return userAssignment is null ? Option.None<UserAssignment>() : Option.Some(userAssignment);
    }

    public async Task<IEnumerable<UserAssignment>> GetByAssignmentIdAsync(AssignmentId assignmentId, CancellationToken cancellation)
    {
        return await UserAssignments
            .Include(x => x.Status)
            .Include(x => x.User)
            .Include(x => x.Assignment)
            .AsNoTracking()
            .Where(x => x.AssignmentId == assignmentId)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserAssignment>> GetByUserIdAndStatusIdAsync(UserId userId, StatusId statusId, CancellationToken cancellation)
    {
        return await UserAssignments
            .Include(x => x.Status)
            .Include(x => x.User)
            .Include(x => x.Assignment)
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.StatusId == statusId)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<UserAssignment>> GetByUserIdAsync(UserId userId, CancellationToken cancellation)
    {
        return await UserAssignments
            .Include(x => x.Status)
            .Include(x => x.User)
            .Include(x => x.Assignment)
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellation);
    }

    public async Task<UserAssignment> Update(UserAssignment userAssignment, CancellationToken cancellation)
    {
        UserAssignments.Update(userAssignment);

        await context.SaveChangesAsync(cancellation);

        return userAssignment;
    }
}
