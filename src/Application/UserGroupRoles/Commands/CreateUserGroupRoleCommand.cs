using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserGroupRoles.Exceptions;
using Domain.UserGroupRoles;
using MediatR;
using Optional.Unsafe;

namespace Application.UserGroupRoles.Commands;

public record CreateUserGroupRoleCommand : IRequest<Result<UserGroupRole, UserGroupRoleException>>
{
    public required string Name { get; init; }
}

public class CreateUserGroupRoleCommandHandler(IUserGroupRoleRepository repository, IUserGroupRoleQueries queries) : IRequestHandler<CreateUserGroupRoleCommand, Result<UserGroupRole, UserGroupRoleException>>
{
    public async Task<Result<UserGroupRole, UserGroupRoleException>> Handle(CreateUserGroupRoleCommand request, CancellationToken cancellationToken)
    {
        var result = (await queries.GetByNameAsync(request.Name, cancellationToken)).ValueOrDefault();

        if (result is not null)
            return new UserGroupRoleAlreadyExistsException(result.Id, request.Name);

        var userGroupRole = UserGroupRole.New(UserGroupRoleId.New(), request.Name);

        return await repository.Create(userGroupRole, cancellationToken);
    }
}

