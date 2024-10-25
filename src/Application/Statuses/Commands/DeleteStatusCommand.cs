using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;

namespace Application.Statuses.Commands;

public class DeleteStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required Guid Id { get; init; }
}

public class DeleteStatusCommandHandler(IStatusRepository repository, IStatusQueries queries) : IRequestHandler<DeleteStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
    {
        var id = new StatusId(request.Id);

        var status = await queries.GetByIdAsync(id, cancellationToken);

        return await status.Match(
            async status =>
            {
                if (status.UserAssignments.Count != 0)
                    return new StatusHasRelationsException(id);

                return await DeleteEntity(status, cancellationToken);
            },
            () => Task.FromResult<Result<Status, StatusException>>(new StatusNotFoundException(id))
        );
    }

    private async Task<Result<Status, StatusException>> DeleteEntity(Status entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new StatusUnknownException(entity.Id, exception);
        }
    }
}
