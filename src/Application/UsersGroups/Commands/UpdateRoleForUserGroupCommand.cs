using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserGroupRoles.Exceptions;
using Application.UsersGroups.Exceptions;
using Domain.Groups;
using Domain.UserGroupRoles;
using Domain.Users;
using Domain.UsersGroups;
using MediatR;

namespace Application.UsersGroups.Commands;

public record UpdateRoleForUserGroupCommand : IRequest<Result<UserGroup, Exception>>
{
    public Guid UserId { get; init; }
    public Guid GroupId { get; init; }
    public Guid UserGroupRoleId { get; init; }
}

public class UpdateRoleForUserGroupCommandHandler(IUserGroupRepository repository, IUserGroupQueries queries, IUserGroupRoleQueries userGroupRoleQueries) : IRequestHandler<UpdateRoleForUserGroupCommand, Result<UserGroup, Exception>>
{
    public async Task<Result<UserGroup, Exception>> Handle(UpdateRoleForUserGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var groupId = new GroupId(request.GroupId);

        var result = await queries.GetByUserIdAndGroupIdAsync(userId, groupId, cancellationToken);

        return await result.Match(
            async userGroup =>
            {
                var userGroupRoleId = new UserGroupRoleId(request.UserGroupRoleId);

                var result = await userGroupRoleQueries.GetByIdAsync(userGroupRoleId, cancellationToken);

                return await result.Match(
                    async userGroupRole => await UpdateEntity(userGroup, userGroupRoleId, cancellationToken),
                    () => Task.FromResult<Result<UserGroup, Exception>>(new UserGroupRoleNotFoundException(userGroupRoleId))
                );
            },
            () => Task.FromResult<Result<UserGroup, Exception>>(new UserGroupNotFoundException(userId, groupId))
        );
    }

    private async Task<Result<UserGroup, Exception>> UpdateEntity(UserGroup userGroup, UserGroupRoleId userGroupRoleId, CancellationToken cancellationToken)
    {
        try
        {
            userGroup.UpdateRole(userGroupRoleId);

            return await repository.Update(userGroup, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserGroupUnknownException(userGroup.UserId, userGroup.GroupId, exception);
        }

    }
}
