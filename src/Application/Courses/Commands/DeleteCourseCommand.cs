using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Courses.Exceptions;
using Domain.Courses;
using MediatR;

namespace Application.Courses.Commands;

public record DeleteCourseCommand : IRequest<Result<Course, CourseException>>
{
    public required Guid Id { get; init; }
}

public class DeleteCourseCommandHandler(ICourseRepository repository, ICourseQueries queries) : IRequestHandler<DeleteCourseCommand, Result<Course, CourseException>>
{
    public async Task<Result<Course, CourseException>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var id = new CourseId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async course =>
            {
                if (course.Assignments.Count != 0)
                    return new CourseHasRelationsException(id);

                return await DeleteEntity(course, cancellationToken);
            },
            () => Task.FromResult<Result<Course, CourseException>>(new CourseNotFoundException(id))
        );
    }

    private async Task<Result<Course, CourseException>> DeleteEntity(Course entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnknownException(entity.Id, exception);
        }
    }
}
