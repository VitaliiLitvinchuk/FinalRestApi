using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.UserRoles.Exceptions;
using Domain.UserRoles;
using MediatR;

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
        var result = await queries.GetByNameAsync(request.Name, cancellationToken);

        return await result.Match(
            userRole => Task.FromResult<Result<UserRole, UserRoleException>>(new UserRoleAlreadyExistsException(userRole.Id, request.Name)),
            async () =>
            {
                var id = new UserRoleId(request.Id);

                var result = await queries.GetByIdAsync(id, cancellationToken);
                return await result.Match(
                    async userRole => await UpdateEntity(userRole, request.Name, cancellationToken),
                    () => Task.FromResult<Result<UserRole, UserRoleException>>(new UserRoleNotFoundException(id))
                );
            }
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

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserRoleUnknownException(entity.Id, exception);
        }
    }
}
