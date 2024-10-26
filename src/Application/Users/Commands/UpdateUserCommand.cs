using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record UpdateUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string AvatarUrl { get; init; }
}

public class UpdateUserCommandHandler(IUserRepository repository, IUserQueries queries) : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var id = new UserId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async user => await UpdateEntity(user, request.FirstName, request.LastName, request.AvatarUrl, cancellationToken),
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(id))
        );
    }

    private async Task<Result<User, UserException>> UpdateEntity(User entity, string firstName, string lastName, string avatarUrl, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetail(firstName, lastName, avatarUrl);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}
