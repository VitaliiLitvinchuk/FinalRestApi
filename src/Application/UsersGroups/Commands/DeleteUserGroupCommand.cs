using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UsersGroups.Exceptions;
using Domain.Groups;
using Domain.Users;
using Domain.UsersGroups;
using MediatR;

namespace Application.UsersGroups.Commands;

public record DeleteUserGroupCommand : IRequest<Result<UserGroup, UserGroupException>>
{
    public required Guid UserId { get; init; }
    public required Guid GroupId { get; init; }
}

public class DeleteUserGroupCommandHandler(IUserGroupRepository repository, IUserGroupQueries queries) : IRequestHandler<DeleteUserGroupCommand, Result<UserGroup, UserGroupException>>
{
    public async Task<Result<UserGroup, UserGroupException>> Handle(DeleteUserGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var groupId = new GroupId(request.GroupId);

        var result = await queries.GetByUserIdAndGroupIdAsync(userId, groupId, cancellationToken);

        return await result.Match(
            async userGroup => await DeleteEntity(userGroup, cancellationToken),
            () => Task.FromResult<Result<UserGroup, UserGroupException>>(new UserGroupNotFoundException(userId, groupId))
        );
    }

    private async Task<Result<UserGroup, UserGroupException>> DeleteEntity(UserGroup userGroup, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(userGroup, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserGroupUnknownException(userGroup.UserId, userGroup.GroupId, exception);
        }
    }
}
