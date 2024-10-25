using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;
using Optional.Unsafe;

namespace Application.Statuses.Commands;

public record CreateStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required string Name { get; init; }
}

public class CreateStatusCommandHandler(IStatusRepository repository, IStatusQueries queries) : IRequestHandler<CreateStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
    {
        var result = (await queries.GetByNameAsync(request.Name, cancellationToken)).ValueOrDefault();

        if (result is not null)
            return new StatusAlreadyExistsException(result.Id, request.Name);

        var status = Status.New(StatusId.New(), request.Name);

        await repository.Create(status, cancellationToken);

        return status;
    }
}
