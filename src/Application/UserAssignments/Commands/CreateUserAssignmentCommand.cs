using Application.Assignments.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserAssignments.Exceptions;
using Application.Users.Exceptions;
using Domain.Assignments;
using Domain.Users;
using Domain.UsersAssignments;
using MediatR;

namespace Application.UserAssignments.Commands;

public record CreateUserAssignmentCommand : IRequest<Result<UserAssignment, Exception>>
{
    public Guid UserId { get; init; }
    public Guid AssignmentId { get; init; }
}

public class CreateUserAssignmentCommandHandler(IUserAssignmentRepository repository, IUserAssignmentQueries queries, IUserQueries userQueries, IAssignmentQueries assignmentQueries, IStatusQueries statusQueries) : IRequestHandler<CreateUserAssignmentCommand, Result<UserAssignment, Exception>>
{
    public async Task<Result<UserAssignment, Exception>> Handle(CreateUserAssignmentCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var userOption = await userQueries.GetByIdAsync(userId, cancellationToken);

        return await userOption.Match(
            async user =>
            {
                var assignmentId = new AssignmentId(request.AssignmentId);

                var assignmentOption = await assignmentQueries.GetByIdAsync(assignmentId, cancellationToken);

                return await assignmentOption.Match(
                    async assignment =>
                    {
                        var userAssignmentOption = await queries.GetByAssignmentIdAndUserIdAsync(userId, assignmentId, cancellationToken);

                        return await userAssignmentOption.Match(
                            userAssignment => Task.FromResult<Result<UserAssignment, Exception>>(new UserAssignmentAlreadyExistsException(userId, assignmentId)),
                            async () =>
                            {
                                var statusOption = await statusQueries.GetByNameAsync("Not Started", cancellationToken);

                                return await statusOption.Match(
                                    async status =>
                                    {
                                        var userAssignment = UserAssignment.New(assignment.Id, user.Id, status.Id);

                                        return await CreateEntity(userAssignment, cancellationToken);
                                    },
                                    () => Task.FromResult<Result<UserAssignment, Exception>>(new DefaultStatusForUserAssignmentException(userId, assignmentId))
                                );
                            }
                        );
                    },
                    () => Task.FromResult<Result<UserAssignment, Exception>>(new AssignmentNotFoundException(assignmentId))
                );
            },
            () => Task.FromResult<Result<UserAssignment, Exception>>(new UserNotFoundException(userId))
        );
    }

    private async Task<Result<UserAssignment, Exception>> CreateEntity(UserAssignment entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserAssignmentUnknownException(entity.UserId, entity.AssignmentId, exception);
        }
    }
}
