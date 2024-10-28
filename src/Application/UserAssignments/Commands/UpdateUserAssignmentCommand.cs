using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserAssignments.Exceptions;
using Domain.Assignments;
using Domain.Statuses;
using Domain.Users;
using Domain.UsersAssignments;
using MediatR;

namespace Application.UserAssignments.Commands;

public record UpdateUserAssignmentCommand : IRequest<Result<UserAssignment, UserAssignmentException>>
{
    public required Guid UserId { get; init; }
    public required Guid AssignmentId { get; init; }
    public required Guid StatusId { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public decimal? Score { get; init; }
}

public class UpdateUserAssignmentCommandHandler(IUserAssignmentRepository repository, IUserAssignmentQueries queries, IStatusQueries statusQueries) : IRequestHandler<UpdateUserAssignmentCommand, Result<UserAssignment, UserAssignmentException>>
{
    public async Task<Result<UserAssignment, UserAssignmentException>> Handle(UpdateUserAssignmentCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var assignmentId = new AssignmentId(request.AssignmentId);

        var userAssignmentOption = await queries.GetByAssignmentIdAndUserIdAsync(userId, assignmentId, cancellationToken);

        return await userAssignmentOption.Match(
            async userAssignment =>
            {
                var statusId = new StatusId(request.StatusId);

                var statusOption = await statusQueries.GetByIdAsync(statusId, cancellationToken);

                return await statusOption.Match(
                    async status => await UpdateEntity(userAssignment, status.Id, request.SubmittedAt, request.Score, cancellationToken),
                    () => Task.FromResult<Result<UserAssignment, UserAssignmentException>>(new StatusForUserAssignmentNotFoundException(userId, assignmentId))
                );
            },
            () => Task.FromResult<Result<UserAssignment, UserAssignmentException>>(new UserAssignmentNotFoundException(userId, assignmentId))
        );
    }

    private async Task<Result<UserAssignment, UserAssignmentException>> UpdateEntity(UserAssignment entity, StatusId statusId, DateTime? submittedAt, decimal? score, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(statusId, submittedAt, score);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserAssignmentUnknownException(entity.UserId, entity.AssignmentId, exception);
        }
    }
}
