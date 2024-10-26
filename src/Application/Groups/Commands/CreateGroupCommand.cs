using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Groups.Exceptions;
using Domain.Groups;
using MediatR;

namespace Application.Groups.Commands;

public record CreateGroupCommand : IRequest<Result<Group, GroupException>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class CreateGroupCommandHandler(IGroupRepository repository) : IRequestHandler<CreateGroupCommand, Result<Group, GroupException>>
{
    public async Task<Result<Group, GroupException>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = Group.New(GroupId.New(), request.Name, request.Description, DateTime.UtcNow);

        return await CreateEntity(group, cancellationToken);
    }

    private async Task<Result<Group, GroupException>> CreateEntity(Group entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new GroupUnknownException(entity.Id, exception);
        }
    }
}
