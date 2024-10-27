using Application.Assignments.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Assignments;
using MediatR;

namespace Application.Assignments.Commands;

public record DeleteAssignmentCommand : IRequest<Result<Assignment, AssignmentException>>
{
    public required Guid Id { get; init; }
}

public class DeleteAssignmentCommandHandler(IAssignmentRepository repository, IAssignmentQueries queries) : IRequestHandler<DeleteAssignmentCommand, Result<Assignment, AssignmentException>>
{
    public async Task<Result<Assignment, AssignmentException>> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var id = new AssignmentId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async assignment =>
            {
                if (assignment.UserAssignments.Count != 0)
                    return new AssignmentHasRelationsException(id);

                return await DeleteEntity(assignment, cancellationToken);
            },
            () => Task.FromResult<Result<Assignment, AssignmentException>>(new AssignmentNotFoundException(id))
        );
    }

    private async Task<Result<Assignment, AssignmentException>> DeleteEntity(Assignment entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new AssignmentUnknownException(entity.Id, exception);
        }
    }
}
