using System.Linq.Expressions;
using Domain.Courses;
using Domain.Groups;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface ICourseQueries
{
    public Task<Option<Course>> GetByIdAsync(CourseId id, CancellationToken cancellation);
    public Task<IEnumerable<Course>> GetByUserIdAsync(UserId userId, CancellationToken cancellation);
    public Task<IEnumerable<Course>> GetByGroupIdAsync(GroupId groupId, CancellationToken cancellation);
    public Task<IEnumerable<Course>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<Course, bool>>? filter = null,
        Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy = null,
        params Expression<Func<Course, object?>>[] includes);
}
