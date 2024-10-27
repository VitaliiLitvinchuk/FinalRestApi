using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UsersGroups.Exceptions;
using Domain.Groups;
using Domain.Users;
using Domain.UsersGroups;
using MediatR;

namespace Application.UsersGroups.Commands;

public record CreateUserGroupCommand : IRequest<Result<UserGroup, UserGroupException>>
{
    public required Guid UserId { get; init; }
    public required Guid GroupId { get; init; }
}

public class CreateUserGroupCommandHandler(IUserGroupRepository repository,
    IUserGroupQueries queries,
    IUserQueries userQueries,
    IGroupQueries groupQueries,
    IUserGroupRoleQueries roleQueries) : IRequestHandler<CreateUserGroupCommand, Result<UserGroup, UserGroupException>>
{
    public async Task<Result<UserGroup, UserGroupException>> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var groupId = new GroupId(request.GroupId);

        var userOption = await userQueries.GetByIdAsync(userId, cancellationToken);

        return await userOption.Match(
            async user =>
            {
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
                                    userGroup => Task.FromResult<Result<UserGroup, UserGroupException>>(new UserGroupAlreadyExistsException(userId, groupId)),
                                    async () => await CreateEntity(UserGroup.New(userId, groupId, userGroupRole.Id, DateTime.UtcNow), cancellationToken)
                                );
                            },
                            () => Task.FromResult<Result<UserGroup, UserGroupException>>(new UserGroupDefaultRoleNotFoundException(userId, groupId))
                        );
                    },
                    () => Task.FromResult<Result<UserGroup, UserGroupException>>(new GroupForUserGroupNotFoundException(userId, groupId))
                );
            },
            () => Task.FromResult<Result<UserGroup, UserGroupException>>(new UserForUserGroupNotFoundException(userId, groupId))
        );
    }

    private async Task<Result<UserGroup, UserGroupException>> CreateEntity(UserGroup userGroup, CancellationToken cancellationToken)
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
