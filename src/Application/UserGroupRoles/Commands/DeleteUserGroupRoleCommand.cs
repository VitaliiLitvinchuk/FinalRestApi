using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserGroupRoles.Exceptions;
using Domain.UserGroupRoles;
using MediatR;

namespace Application.UserGroupRoles.Commands;

public record DeleteUserGroupRoleCommand : IRequest<Result<UserGroupRole, UserGroupRoleException>>
{
    public required Guid Id { get; init; }
}

public class DeleteUserGroupRoleCommandHandler(IUserGroupRoleRepository repository, IUserGroupRoleQueries queries) : IRequestHandler<DeleteUserGroupRoleCommand, Result<UserGroupRole, UserGroupRoleException>>
{
    public async Task<Result<UserGroupRole, UserGroupRoleException>> Handle(DeleteUserGroupRoleCommand request, CancellationToken cancellationToken)
    {
        var id = new UserGroupRoleId(request.Id);

        var userGroupRole = await queries.GetByIdAsync(id, cancellationToken);

        return await userGroupRole.Match(
            async userGroupRole =>
            {
                if (userGroupRole.UserGroups.Count != 0)
                    return new UserGroupRoleHasRelationsException(id);

                return await DeleteEntity(userGroupRole, cancellationToken);
            },
            () => Task.FromResult<Result<UserGroupRole, UserGroupRoleException>>(new UserGroupRoleNotFoundException(id))
        );
    }

    private async Task<Result<UserGroupRole, UserGroupRoleException>> DeleteEntity(UserGroupRole entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserGroupRoleUnknownException(entity.Id, exception);
        }
    }
}
