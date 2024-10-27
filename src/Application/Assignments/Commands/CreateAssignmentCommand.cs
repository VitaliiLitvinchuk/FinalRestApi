using Application.Assignments.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Assignments;
using Domain.Courses;
using MediatR;

namespace Application.Assignments.Commands;

public record CreateAssignmentCommand : IRequest<Result<Assignment, AssignmentException>>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DateTime DueDate { get; init; }
    public required decimal MaxScore { get; init; }
    public required Guid CourseId { get; init; }
}

public class CreateAssignmentCommandHandler(IAssignmentRepository repository, ICourseQueries courseQueries) : IRequestHandler<CreateAssignmentCommand, Result<Assignment, AssignmentException>>
{
    public async Task<Result<Assignment, AssignmentException>> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var id = AssignmentId.New();
        var courseId = new CourseId(request.CourseId);

        var courseOption = await courseQueries.GetByIdAsync(courseId, cancellationToken);

        return await courseOption.Match(
            async course => await CreateEntity(Assignment.New(AssignmentId.New(), courseId, request.Title, request.Description, request.DueDate, request.MaxScore, DateTime.UtcNow), cancellationToken),
            () => Task.FromResult<Result<Assignment, AssignmentException>>(new CourseForAssignmentNotFoundException(id, courseId))
        );
    }

    private async Task<Result<Assignment, AssignmentException>> CreateEntity(Assignment entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new AssignmentUnknownException(entity.Id, exception);
        }
    }
}
