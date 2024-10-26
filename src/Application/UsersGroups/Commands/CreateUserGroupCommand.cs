using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Groups.Exceptions;
using Application.Users.Exceptions;
using Application.UsersGroups.Exceptions;
using Domain.Groups;
using Domain.Users;
using Domain.UsersGroups;
using MediatR;

namespace Application.UsersGroups.Commands;

public record CreateUserGroupCommand : IRequest<Result<UserGroup, Exception>>
{
    public required Guid UserId { get; init; }
    public required Guid GroupId { get; init; }
}

public class CreateUserGroupCommandHandler(IUserGroupRepository repository,
    IUserGroupQueries queries,
    IUserQueries userQueries,
    IGroupQueries groupQueries,
    IUserGroupRoleQueries roleQueries) : IRequestHandler<CreateUserGroupCommand, Result<UserGroup, Exception>>
{
    public async Task<Result<UserGroup, Exception>> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var userOption = await userQueries.GetByIdAsync(userId, cancellationToken);

        return await userOption.Match(
            async user =>
            {
                var groupId = new GroupId(request.GroupId);
                var groupOption = await groupQueries.GetByIdAsync(groupId, cancellationToken);

                return await groupOption.Match(
                    async group =>
                    {
                        var userGroupRoleOption = await roleQueries.GetByNameAsync("Member", cancellationToken);

                        return await userGroupRoleOption.Match(
                            async userGroupRole =>
                            {
                                var userGroupOption = await queries.GetByUserIdAndGroupIdAsync(userId, groupId, cancellationToken);

                                return await userGroupOption.Match(
                                    userGroup => Task.FromResult<Result<UserGroup, Exception>>(new UserGroupAlreadyExistsException(userId, groupId)),
                                    async () => await CreateEntity(UserGroup.New(userId, groupId, userGroupRole.Id, DateTime.UtcNow), cancellationToken)
                                );
                            },
                            () => Task.FromResult<Result<UserGroup, Exception>>(new UserGroupDefaultRoleNotFoundException(userId, groupId))
                        );
                    },
                    () => Task.FromResult<Result<UserGroup, Exception>>(new GroupNotFoundException(groupId))
                );
            },
            () => Task.FromResult<Result<UserGroup, Exception>>(new UserNotFoundException(userId))
        );
    }

    private async Task<Result<UserGroup, Exception>> CreateEntity(UserGroup userGroup, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(userGroup, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserGroupUnknownException(userGroup.UserId, userGroup.GroupId, exception);
        }
    }
}
