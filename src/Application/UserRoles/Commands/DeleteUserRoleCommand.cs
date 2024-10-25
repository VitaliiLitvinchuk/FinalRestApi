using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserRoles.Exceptions;
using Domain.UserRoles;
using MediatR;
using Optional.Unsafe;

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

        var userRole = (await queries.GetByIdAsync(id, cancellationToken)).ValueOrDefault();

        if (userRole is null)
            return new UserRoleNotFoundException(id);

        if (userRole.Users.Count != 0)
            return new UserRoleHasRelationsException(id);

        return await repository.Delete(userRole, cancellationToken);
    }
}
