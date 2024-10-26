using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Groups.Exceptions;
using Domain.Groups;
using MediatR;

namespace Application.Groups.Commands;

public record UpdateGroupCommand : IRequest<Result<Group, GroupException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class UpdateGroupCommandHandler(IGroupRepository repository, IGroupQueries queries) : IRequestHandler<UpdateGroupCommand, Result<Group, GroupException>>
{
    public async Task<Result<Group, GroupException>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var id = new GroupId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async group => await UpdateEntity(group, request.Name, request.Description, cancellationToken),
            () => Task.FromResult<Result<Group, GroupException>>(new GroupNotFoundException(id))
        );
    }

    private async Task<Result<Group, GroupException>> UpdateEntity(Group entity, string name, string description, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name, description);
            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new GroupUnknownException(entity.Id, exception);
        }
    }
}
