using Application.Assignments.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Assignments;
using MediatR;

namespace Application.Assignments.Commands;

public record UpdateAssignmentCommand : IRequest<Result<Assignment, AssignmentException>>
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DateTime DueDate { get; init; }
    public required int MaxScore { get; init; }
}

public class UpdateAssignmentCommandHandler(IAssignmentRepository repository, IAssignmentQueries queries) : IRequestHandler<UpdateAssignmentCommand, Result<Assignment, AssignmentException>>
{
    public async Task<Result<Assignment, AssignmentException>> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var id = new AssignmentId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async assignment => await UpdateEntity(assignment, request.Title, request.Description, request.DueDate, request.MaxScore, cancellationToken),
            () => Task.FromResult<Result<Assignment, AssignmentException>>(new AssignmentNotFoundException(id))
        );
    }

    private async Task<Result<Assignment, AssignmentException>> UpdateEntity(Assignment entity, string title, string description, DateTime dueDate, int maxScore, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(title, description, dueDate, maxScore);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new AssignmentUnknownException(entity.Id, exception);
        }
    }
}
