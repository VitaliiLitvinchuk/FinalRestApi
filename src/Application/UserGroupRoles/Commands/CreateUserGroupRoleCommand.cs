using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserGroupRoles.Exceptions;
using Domain.UserGroupRoles;
using MediatR;

namespace Application.UserGroupRoles.Commands;

public record CreateUserGroupRoleCommand : IRequest<Result<UserGroupRole, UserGroupRoleException>>
{
    public required string Name { get; init; }
}

public class CreateUserGroupRoleCommandHandler(IUserGroupRoleRepository repository, IUserGroupRoleQueries queries) : IRequestHandler<CreateUserGroupRoleCommand, Result<UserGroupRole, UserGroupRoleException>>
{
    public async Task<Result<UserGroupRole, UserGroupRoleException>> Handle(CreateUserGroupRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await queries.GetByNameAsync(request.Name, cancellationToken);

        return await result.Match(
            userGroupRole => Task.FromResult<Result<UserGroupRole, UserGroupRoleException>>(new UserGroupRoleAlreadyExistsException(userGroupRole.Id, request.Name)),
            async () => await CreateEntity(UserGroupRole.New(UserGroupRoleId.New(), request.Name), cancellationToken)
        );
    }

    private async Task<Result<UserGroupRole, UserGroupRoleException>> CreateEntity(UserGroupRole entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserGroupRoleUnknownException(entity.Id, exception);
        }
    }
}

