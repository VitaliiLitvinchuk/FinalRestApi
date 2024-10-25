using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserRoles.Exceptions;
using Domain.UserRoles;
using MediatR;

namespace Application.UserRoles.Commands;

public record DeleteUserRoleCommand : IRequest<Result<UserRole, UserRoleException>>
{
    public required Guid Id { get; init; }
}

public class DeleteUserRoleCommandHandler(IUserRoleRepository repository, IUserRoleQueries queries) : IRequestHandler<DeleteUserRoleCommand, Result<UserRole, UserRoleException>>
{
    public async Task<Result<UserRole, UserRoleException>> Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
    {
        var id = new UserRoleId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async userRole =>
            {
                if (userRole.Users.Count != 0)
                    return new UserRoleHasRelationsException(id);

                return await DeleteEntity(userRole, cancellationToken);
            },
            () => Task.FromResult<Result<UserRole, UserRoleException>>(new UserRoleNotFoundException(id))
        );
    }

    private async Task<Result<UserRole, UserRoleException>> DeleteEntity(UserRole entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserRoleUnknownException(entity.Id, exception);
        }
    }
}
