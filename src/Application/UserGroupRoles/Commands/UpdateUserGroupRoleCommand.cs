using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserGroupRoles.Exceptions;
using Domain.UserGroupRoles;
using MediatR;

namespace Application.UserGroupRoles.Commands;

public record UpdateUserGroupRoleCommand : IRequest<Result<UserGroupRole, UserGroupRoleException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class UpdateUserGroupRoleCommandHandler(IUserGroupRoleRepository repository, IUserGroupRoleQueries queries) : IRequestHandler<UpdateUserGroupRoleCommand, Result<UserGroupRole, UserGroupRoleException>>
{
    public async Task<Result<UserGroupRole, UserGroupRoleException>> Handle(UpdateUserGroupRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await queries.GetByNameAsync(request.Name, cancellationToken);

        return await result.Match(
            userGroupRole => Task.FromResult<Result<UserGroupRole, UserGroupRoleException>>(new UserGroupRoleAlreadyExistsException(userGroupRole.Id, request.Name)),
            async () =>
            {
                var id = new UserGroupRoleId(request.Id);

                var result = await queries.GetByIdAsync(id, cancellationToken);

                return await result.Match(
                    async userGroupRole => await UpdateEntity(userGroupRole, request.Name, cancellationToken),
                    () => Task.FromResult<Result<UserGroupRole, UserGroupRoleException>>(new UserGroupRoleNotFoundException(id))
                );
            }
        );
    }

    private async Task<Result<UserGroupRole, UserGroupRoleException>> UpdateEntity(
        UserGroupRole entity,
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserGroupRoleUnknownException(entity.Id, exception);
        }
    }
}
