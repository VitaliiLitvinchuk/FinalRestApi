using Domain.UserRoles;

namespace Application.UserRoles.Exceptions;

public abstract class UserRoleException(UserRoleId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public UserRoleId Id { get; } = id;
}

public class UserRoleNotFoundException(UserRoleId id) : UserRoleException(id, $"User role with id: {id} not found.");
public class UserRoleAlreadyExistsException(UserRoleId id, string name) : UserRoleException(id, $"User role with name: {name} already exists.");
public class UserRoleUnknownException(UserRoleId id, Exception innerException) : UserRoleException(id, $"User role with id: {id} is unknown.", innerException);
public class UserRoleHasRelationsException(UserRoleId id) : UserRoleException(id, $"User role with id: {id} has relations.");