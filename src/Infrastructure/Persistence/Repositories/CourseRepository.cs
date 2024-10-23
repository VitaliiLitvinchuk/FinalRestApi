using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Courses;
using Domain.Groups;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class CourseRepository(ApplicationDbContext context) : ICourseRepository, ICourseQueries
{
    private DbSet<Course> Courses { get; } = context.Courses;
    public async Task<Course> Create(Course course, CancellationToken cancellation)
    {
        await Courses.AddAsync(course, cancellation);

        await context.SaveChangesAsync(cancellation);

        return course;
    }

    public async Task<Course> Delete(Course course, CancellationToken cancellation)
    {
        Courses.Remove(course);

        await context.SaveChangesAsync(cancellation);

        return course;
    }

    public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellation)
    {
        return await Courses
            .Include(x => x.Group)
            .Include(x => x.User)
            .Include(x => x.Assignments)
            .AsNoTracking()
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<Course>> GetByGroupIdAsync(GroupId groupId, CancellationToken cancellation)
    {
        return await Courses
            .Include(x => x.Group)
            .Include(x => x.User)
            .Include(x => x.Assignments)
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .ToListAsync(cancellation);
    }

    public async Task<Option<Course>> GetByIdAsync(CourseId id, CancellationToken cancellation)
    {
        var course = await Courses
            .Include(x => x.Group)
            .Include(x => x.User)
            .Include(x => x.Assignments)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        return course is null ? Option.None<Course>() : Option.Some(course);
    }

    public async Task<IEnumerable<Course>> GetByUserIdAsync(UserId userId, CancellationToken cancellation)
    {
        return await Courses
            .Include(x => x.Group)
            .Include(x => x.User)
            .Include(x => x.Assignments)
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellation);
    }

    public async Task<Course> Update(Course course, CancellationToken cancellation)
    {
        Courses.Update(course);

        await context.SaveChangesAsync(cancellation);

        return course;
    }
}
