using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;

namespace Application.Statuses.Commands;

public record CreateStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required string Name { get; set; }
}

public class CreateStatusCommandHandler(IStatusRepository repository, IStatusQueries queries) : IRequestHandler<CreateStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
    {
        var id = StatusId.New();

        var result = await queries.GetByNameAsync(request.Name, cancellationToken);

        if (result.HasValue)
            return new StatusAlreadyExistsException(id, request.Name);

        var status = Status.New(id, request.Name);

        await repository.Create(status, cancellationToken);

        return status;
    }
}
