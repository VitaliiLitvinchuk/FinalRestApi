using Application.Assignments.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Assignments;
using Domain.Courses;
using MediatR;

namespace Application.Assignments.Commands;

public record UpdateCourseForAssignmentCommand : IRequest<Result<Assignment, AssignmentException>>
{
    public required Guid Id { get; init; }
    public required Guid CourseId { get; init; }
}

public class UpdateCourseForAssignmentCommandHandler(IAssignmentRepository repository, IAssignmentQueries queries, ICourseQueries courseQueries) : IRequestHandler<UpdateCourseForAssignmentCommand, Result<Assignment, AssignmentException>>
{
    public async Task<Result<Assignment, AssignmentException>> Handle(UpdateCourseForAssignmentCommand request, CancellationToken cancellationToken)
    {
        var id = new AssignmentId(request.Id);

        var assignmentOption = await queries.GetByIdAsync(id, cancellationToken);

        return await assignmentOption.Match(
            async entity =>
            {
                var courseId = new CourseId(request.CourseId);

                var courseOption = await courseQueries.GetByIdAsync(courseId, cancellationToken);

                return await courseOption.Match(
                    async course => await UpdateCourse(entity, course.Id, cancellationToken),
                    () => Task.FromResult<Result<Assignment, AssignmentException>>(new CourseForAssignmentNotFoundException(id, courseId))
                );
            },
            () => Task.FromResult<Result<Assignment, AssignmentException>>(new AssignmentNotFoundException(id))
        );
    }

    private async Task<Result<Assignment, AssignmentException>> UpdateCourse(Assignment entity, CourseId courseId, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateCourse(courseId);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new AssignmentUnknownException(entity.Id, exception);
        }
    }
}
