using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;

namespace Application.Statuses.Commands;

public class DeleteStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required Guid Id { get; set; }
}

public class DeleteStatusCommandHandler(IStatusRepository repository, IStatusQueries queries) : IRequestHandler<DeleteStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
    {
        var id = new StatusId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match<Task<Result<Status, StatusException>>>(
            async status => await repository.Delete(status, cancellationToken),
            () => Task.FromResult<Result<Status, StatusException>>(new StatusNotFoundException(id))
        );
    }
}
