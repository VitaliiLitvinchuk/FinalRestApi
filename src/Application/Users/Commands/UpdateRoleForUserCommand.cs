using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.UserRoles;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record UpdateRoleForUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid Id { get; init; }
    public required Guid UserRoleId { get; init; }
}

public class UpdateRoleForUserCommandHandler(IUserRepository repository, IUserQueries queries, IUserRoleQueries roleQueries) : IRequestHandler<UpdateRoleForUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateRoleForUserCommand request, CancellationToken cancellationToken)
    {
        var id = new UserId(request.Id);
        var userRoleId = new UserRoleId(request.UserRoleId);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async user =>
            {
                var userRole = await roleQueries.GetByIdAsync(userRoleId, cancellationToken);

                return await userRole.Match(
                    async userRole => await UpdateEntity(user, userRole, cancellationToken),
                    () => Task.FromResult<Result<User, UserException>>(new RoleForUserNotFoundException(id, userRoleId))
                );
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(id))
        );
    }

    private async Task<Result<User, UserException>> UpdateEntity(User entity, UserRole userRole, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateRole(userRole.Id);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}
