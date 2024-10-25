using Domain.UserGroupRoles;

namespace Application.UserGroupRoles.Exceptions;

public class UserGroupRoleException(UserGroupRoleId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public UserGroupRoleId Id { get; } = id;
}

public class UserGroupRoleNotFoundException(UserGroupRoleId id) : UserGroupRoleException(id, $"User group role with id: {id} not found.");
public class UserGroupRoleAlreadyExistsException(UserGroupRoleId id, string name) : UserGroupRoleException(id, $"User group role with name: {name} already exists.");
public class UserGroupRoleHasRelationsException(UserGroupRoleId id) : UserGroupRoleException(id, $"User group role with id: {id} has relations.");
public class UserGroupRoleUnknownException(UserGroupRoleId id, Exception innerException) : UserGroupRoleException(id, $"User group role with id: {id} is unknown.", innerException);