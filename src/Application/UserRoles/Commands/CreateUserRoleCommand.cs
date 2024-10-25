using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserRoles.Exceptions;
using Domain.UserRoles;
using MediatR;

namespace Application.UserRoles.Commands;

public record CreateUserRoleCommand : IRequest<Result<UserRole, UserRoleException>>
{
    public required string Name { get; init; }
}

public class CreateUserRoleCommandHandler(IUserRoleRepository repository, IUserRoleQueries queries) : IRequestHandler<CreateUserRoleCommand, Result<UserRole, UserRoleException>>
{
    public async Task<Result<UserRole, UserRoleException>> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await queries.GetByNameAsync(request.Name, cancellationToken);

        return await result.Match(
            userRole => Task.FromResult<Result<UserRole, UserRoleException>>(new UserRoleAlreadyExistsException(userRole.Id, request.Name)),
            async () => await CreateEntity(UserRole.New(UserRoleId.New(), request.Name), cancellationToken)
        );
    }

    private async Task<Result<UserRole, UserRoleException>> CreateEntity(UserRole entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserRoleUnknownException(entity.Id, exception);
        }
    }
}