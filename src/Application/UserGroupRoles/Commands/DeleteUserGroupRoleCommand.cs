using System;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserGroupRoles.Exceptions;
using Domain.UserGroupRoles;
using MediatR;
using Optional.Unsafe;

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

        var userGroupRole = (await queries.GetByIdAsync(id, cancellationToken)).ValueOrDefault();

        if (userGroupRole is null)
            return new UserGroupRoleNotFoundException(id);

        if (userGroupRole.UserGroups.Count != 0)
            return new UserGroupRoleHasRelationsException(id);

        return await repository.Delete(userGroupRole, cancellationToken);
    }
}
