using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserRoles.Exceptions;
using Domain.UserRoles;
using MediatR;
using Optional.Unsafe;

namespace Application.UserRoles.Commands;

public record CreateUserRoleCommand : IRequest<Result<UserRole, UserRoleException>>
{
    public required string Name { get; init; }
}

public class CreateUserRoleCommandHandler(IUserRoleRepository repository, IUserRoleQueries queries) : IRequestHandler<CreateUserRoleCommand, Result<UserRole, UserRoleException>>
{
    public async Task<Result<UserRole, UserRoleException>> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var result = (await queries.GetByNameAsync(request.Name, cancellationToken)).ValueOrDefault();

        if (result is not null)
            return new UserRoleAlreadyExistsException(result.Id, request.Name);

        var userRole = UserRole.New(UserRoleId.New(), request.Name);

        await repository.Create(userRole, cancellationToken);

        return userRole;
    }
}