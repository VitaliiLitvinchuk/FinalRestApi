using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserAssignments.Exceptions;
using Domain.Assignments;
using Domain.Users;
using Domain.UsersAssignments;
using MediatR;

namespace Application.UserAssignments.Commands;

public record DeleteUserAssignmentCommand : IRequest<Result<UserAssignment, UserAssignmentException>>
{
    public required Guid UserId { get; init; }
    public required Guid AssignmentId { get; init; }
}

public class DeleteUserAssignmentCommandHandler(IUserAssignmentRepository repository, IUserAssignmentQueries queries) : IRequestHandler<DeleteUserAssignmentCommand, Result<UserAssignment, UserAssignmentException>>
{
    public async Task<Result<UserAssignment, UserAssignmentException>> Handle(DeleteUserAssignmentCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var assignmentId = new AssignmentId(request.AssignmentId);

        var userAssignmentOption = await queries.GetByAssignmentIdAndUserIdAsync(userId, assignmentId, cancellationToken);

        return await userAssignmentOption.Match(
            async userAssignment => await DeleteEntity(userAssignment, cancellationToken),
            () => Task.FromResult<Result<UserAssignment, UserAssignmentException>>(new UserAssignmentNotFoundException(userId, assignmentId))
        );
    }

    private async Task<Result<UserAssignment, UserAssignmentException>> DeleteEntity(UserAssignment entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserAssignmentUnknownException(entity.UserId, entity.AssignmentId, exception);
        }
    }
}
