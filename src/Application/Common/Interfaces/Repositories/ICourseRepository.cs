using Domain.Courses;

namespace Application.Common.Interfaces.Repositories;

public interface ICourseRepository
{
    public Task<Course> Create(Course course, CancellationToken cancellation);
    public Task<Course> Update(Course course, CancellationToken cancellation);
    public Task<Course> Delete(Course course, CancellationToken cancellation);
}
