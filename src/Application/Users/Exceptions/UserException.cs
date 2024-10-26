using Domain.UserRoles;
using Domain.Users;

namespace Application.Users.Exceptions;

public abstract class UserException(UserId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public UserId Id { get; } = id;
}

public class UserNotFoundException(UserId id) : UserException(id, $"User with id: {id} not found.");
public class UserAlreadyExistsException(UserId id, string email) : UserException(id, $"User with email: {email} already exists.");
public class UserUnknownException(UserId id, Exception innerException) : UserException(id, $"User with id: {id} is unknown.", innerException);
public class UserHasRelationsException(UserId id) : UserException(id, $"User with id: {id} has relations.");
public class UserDefaultRoleNotFoundException(UserId id) : UserException(id, $"Default role not found for user with id: {id}.");
public class RoleForUserNotFoundException(UserId id, UserRoleId userRoleId) : UserException(id, $"Role with id: {userRoleId} for user with id: {id} not found.");