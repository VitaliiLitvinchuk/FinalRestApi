using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Assignments;
using Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class AssignmentRepository(ApplicationDbContext context) : IAssignmentRepository, IAssignmentQueries
{
    private DbSet<Assignment> Assignments { get; } = context.Assignments;

    public async Task<Assignment> Create(Assignment assignment, CancellationToken cancellation)
    {
        await Assignments.AddAsync(assignment, cancellation);

        await context.SaveChangesAsync(cancellation);

        return (await GetByIdAsync(assignment.Id, cancellation)).Match(x => x, () => throw new Exception("Could not create assignment"));
    }

    public async Task<Assignment> Delete(Assignment assignment, CancellationToken cancellation)
    {
        Assignments.Remove(assignment);

        await context.SaveChangesAsync(cancellation);

        return assignment;
    }

    public async Task<IEnumerable<Assignment>> GetCourseIdAsync(CourseId id, CancellationToken cancellation)
    {
        return await Assignments
            .Include(x => x.UserAssignments)
            .Include(x => x.Course)
            .AsNoTracking()
            .Where(x => x.CourseId == id)
            .ToListAsync(cancellation);
    }

    public async Task<Option<Assignment>> GetByIdAsync(AssignmentId id, CancellationToken cancellation)
    {
        var assignment = await Assignments
            .Include(x => x.UserAssignments)
            .Include(x => x.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        return assignment is null ? Option.None<Assignment>() : Option.Some(assignment);
    }

    public async Task<Assignment> Update(Assignment assignment, CancellationToken cancellation)
    {
        Assignments.Update(assignment);

        await context.SaveChangesAsync(cancellation);

        return (await GetByIdAsync(assignment.Id, cancellation)).Match(x => x, () => throw new Exception("Could not update assignment"));
    }

    public async Task<IEnumerable<Assignment>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<Assignment, bool>>? filter = null,
        Func<IQueryable<Assignment>, IOrderedQueryable<Assignment>>? orderBy = null,
        params Expression<Func<Assignment, object?>>[] includes)
    {
        IQueryable<Assignment> query = Assignments.AsQueryable()
                                                  .AsNoTracking();

        if (filter != null)
            query = query.Where(filter);

        foreach (var include in includes)
            query = query.Include(include);

        if (orderBy != null)
            return await orderBy(query).ToListAsync(cancellation);

        return await query.ToListAsync(cancellation);
    }
}
