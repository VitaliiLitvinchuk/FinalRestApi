using Domain.Groups;

namespace Application.Groups.Exceptions;

public abstract class GroupException(GroupId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public GroupId Id { get; } = id;
}

public class GroupNotFoundException(GroupId id) : GroupException(id, $"Group with id: {id} not found.");
public class GroupUnknownException(GroupId id, Exception innerException) : GroupException(id, $"Group with id: {id} is unknown.", innerException);
public class GroupHasRelationsException(GroupId id) : GroupException(id, $"Group with id: {id} has relations.");