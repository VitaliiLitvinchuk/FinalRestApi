using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserRoles.Exceptions;
using Domain.UserRoles;
using MediatR;
using Optional.Unsafe;

namespace Application.UserRoles.Commands;

public record UpdateUserRoleCommand : IRequest<Result<UserRole, UserRoleException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class UpdateUserRoleCommandHandler(IUserRoleRepository repository, IUserRoleQueries queries) : IRequestHandler<UpdateUserRoleCommand, Result<UserRole, UserRoleException>>
{
    public async Task<Result<UserRole, UserRoleException>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var result = (await queries.GetByNameAsync(request.Name, cancellationToken)).ValueOrDefault();

        if (result is not null)
            return new UserRoleAlreadyExistsException(result.Id, request.Name);

        var id = new UserRoleId(request.Id);

        var exist = await queries.GetByIdAsync(id, cancellationToken);

        return await exist.Match(
            async userRole => await UpdateEntity(userRole, request.Name, cancellationToken),
            () => Task.FromResult<Result<UserRole, UserRoleException>>(new UserRoleNotFoundException(id))
        );
    }

    private async Task<Result<UserRole, UserRoleException>> UpdateEntity(
        UserRole entity,
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name);
            await repository.Update(entity, cancellationToken);
            return entity;
        }
        catch (Exception exception)
        {
            return new UserRoleUnknownException(entity.Id, exception);
        }
    }
}
