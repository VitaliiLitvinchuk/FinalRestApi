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

public record UpdateUserAssignmentCommand : IRequest<Result<UserAssignment, Exception>>
{
    public required Guid UserId { get; init; }
    public required Guid AssignmentId { get; init; }
    public Guid StatusId { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public decimal? Score { get; init; }
}

public class UpdateUserAssignmentCommandHandler(IUserAssignmentRepository repository, IUserAssignmentQueries queries, IStatusQueries statusQueries) : IRequestHandler<UpdateUserAssignmentCommand, Result<UserAssignment, Exception>>
{
    public async Task<Result<UserAssignment, Exception>> Handle(UpdateUserAssignmentCommand request, CancellationToken cancellationToken)
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
                    () => Task.FromResult<Result<UserAssignment, Exception>>(new StatusForUserAssignmentException(userId, assignmentId))
                );
            },
            () => Task.FromResult<Result<UserAssignment, Exception>>(new UserAssignmentNotFoundException(userId, assignmentId))
        );
    }

    private async Task<Result<UserAssignment, Exception>> UpdateEntity(UserAssignment entity, StatusId statusId, DateTime? submittedAt, decimal? score, CancellationToken cancellationToken)
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
