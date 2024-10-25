using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;
using Optional.Unsafe;

namespace Application.Statuses.Commands;

public record UpdateStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class UpdateStatusCommandHandler(IStatusRepository repository, IStatusQueries queries) : IRequestHandler<UpdateStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var result = (await queries.GetByNameAsync(request.Name, cancellationToken)).ValueOrDefault();

        if (result is not null)
            return new StatusAlreadyExistsException(result.Id, request.Name);

        var id = new StatusId(request.Id);

        var exist = await queries.GetByIdAsync(id, cancellationToken);

        return await exist.Match(
            async status => await UpdateEntity(status, request.Name, cancellationToken),
            () => Task.FromResult<Result<Status, StatusException>>(new StatusNotFoundException(id))
        );
    }


    private async Task<Result<Status, StatusException>> UpdateEntity(
        Status entity,
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new StatusUnknownException(entity.Id, exception);
        }
    }
}