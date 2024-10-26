using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<Result<User, UserException>>
{
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string AvatarUrl { get; init; }
    public required string GoogleId { get; init; }
}

public class CreateUserCommandHandler(IUserRepository repository, IUserQueries queries, IUserRoleQueries roleQueries) : IRequestHandler<CreateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await queries.GetByEmailAsync(request.Email, cancellationToken);

        return await result.Match(
            user => Task.FromResult<Result<User, UserException>>(new UserAlreadyExistsException(user.Id, request.Email)),
            async () =>
            {
                var id = UserId.New();
                var role = await roleQueries.GetByNameAsync("User", cancellationToken);

                return await role.Match(
                    async role => await CreateEntity(User.New(id, request.FirstName, request.LastName, request.Email, request.GoogleId, request.AvatarUrl, role.Id, DateTime.UtcNow), cancellationToken),
                    () => Task.FromResult<Result<User, UserException>>(new UserDefaultRoleNotFoundException(id))
                );
            }
        );
    }

    private async Task<Result<User, UserException>> CreateEntity(User entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}
