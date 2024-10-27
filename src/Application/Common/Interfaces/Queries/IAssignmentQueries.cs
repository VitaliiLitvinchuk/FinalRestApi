using Domain.Assignments;
using Domain.Courses;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IAssignmentQueries
{
    public Task<Option<Assignment>> GetByIdAsync(AssignmentId id, CancellationToken cancellation);
    public Task<IEnumerable<Assignment>> GetCourseIdAsync(CourseId id, CancellationToken cancellation);
    public Task<IEnumerable<Assignment>> GetAllAsync(CancellationToken cancellation);
}
