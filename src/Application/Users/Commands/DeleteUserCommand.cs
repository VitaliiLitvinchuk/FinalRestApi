using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record DeleteUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid Id { get; init; }
}

public class DeleteUserCommandHandler(IUserRepository repository, IUserQueries queries) : IRequestHandler<DeleteUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var result = await queries.GetByIdAsync(new UserId(request.Id), cancellationToken);

        return await result.Match(
            async user =>
            {
                if (user.UserGroups.Count != 0 || user.UserAssignments.Count != 0 || user.Courses.Count != 0)
                    return new UserHasRelationsException(user.Id);

                return await DeleteEntity(user, cancellationToken);
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(new UserId(request.Id)))
        );
    }

    private async Task<Result<User, UserException>> DeleteEntity(User entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}
