using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;

namespace Application.Statuses.Commands;

public record CreateStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required string Name { get; init; }
}

public class CreateStatusCommandHandler(IStatusRepository repository, IStatusQueries queries) : IRequestHandler<CreateStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
    {
        var result = await queries.GetByNameAsync(request.Name, cancellationToken);

        return await result.Match(
            status => Task.FromResult<Result<Status, StatusException>>(new StatusAlreadyExistsException(status.Id, request.Name)),
            async () => await CreateEntity(Status.New(StatusId.New(), request.Name), cancellationToken)
        );
    }

    private async Task<Result<Status, StatusException>> CreateEntity(Status entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new StatusUnknownException(entity.Id, exception);
        }
    }
}
