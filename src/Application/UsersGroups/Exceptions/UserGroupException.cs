using Domain.Groups;
using Domain.Users;

namespace Application.UsersGroups.Exceptions;

public abstract class UserGroupException(UserId userId, GroupId groupId, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public UserId UserId { get; } = userId;
    public GroupId GroupId { get; } = groupId;
}

public class UserGroupNotFoundException(UserId userId, GroupId groupId) : UserGroupException(userId, groupId, $"User with id: {userId} and group with id: {groupId} relation not found.");
public class UserGroupAlreadyExistsException(UserId userId, GroupId groupId) : UserGroupException(userId, groupId, $"User with id: {userId} and group with id: {groupId} relation already exists.");
public class UserGroupUnknownException(UserId userId, GroupId groupId, Exception innerException) : UserGroupException(userId, groupId, $"User with id: {userId} and group with id: {groupId} relation is unknown.", innerException);
public class UserGroupDefaultRoleNotFoundException(UserId userId, GroupId groupId) : UserGroupException(userId, groupId, $"Default role not found for user with id: {userId} and group with id: {groupId}.");
public class RoleForUserGroupNotFoundException(UserId userId, GroupId groupId) : UserGroupException(userId, groupId, $"Role for user with id: {userId} and group with id: {groupId} not found.");
public class GroupForUserGroupNotFoundException(UserId userId, GroupId groupId) : UserGroupException(userId, groupId, $"Group with id: {groupId} not found.");
public class UserForUserGroupNotFoundException(UserId userId, GroupId groupId) : UserGroupException(userId, groupId, $"User with id: {userId} not found.");