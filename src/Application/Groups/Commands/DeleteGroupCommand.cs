using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Groups.Exceptions;
using Domain.Groups;
using MediatR;

namespace Application.Groups.Commands;

public record DeleteGroupCommand : IRequest<Result<Group, GroupException>>
{
    public required Guid Id { get; init; }
}

public class DeleteGroupCommandHandler(IGroupRepository repository, IGroupQueries queries) : IRequestHandler<DeleteGroupCommand, Result<Group, GroupException>>
{
    public async Task<Result<Group, GroupException>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var id = new GroupId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async group =>
            {
                if (group.Courses.Count != 0 || group.UserGroups.Count != 0)
                    return new GroupHasRelationsException(id);

                return await DeleteEntity(group, cancellationToken);
            },
            () => Task.FromResult<Result<Group, GroupException>>(new GroupNotFoundException(id))
        );
    }

    private async Task<Result<Group, GroupException>> DeleteEntity(Group entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new GroupUnknownException(entity.Id, exception);
        }
    }
}
