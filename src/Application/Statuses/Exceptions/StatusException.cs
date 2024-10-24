using Domain.Statuses;

namespace Application.Statuses.Exceptions;

public abstract class StatusException(StatusId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public StatusId Id { get; } = id;
}

public class StatusNotFoundException(StatusId id) : StatusException(id, $"Status with id: {id} not found.");
public class StatusAlreadyExistsException(StatusId id, string name) : StatusException(id, $"Status with name: {name} already exists.");
public class StatusUnknownException(StatusId id, Exception innerException) : StatusException(id, $"Status with id: {id} is unknown.", innerException);
public class StatusHasRelationsException(StatusId id) : StatusException(id, $"Status with id: {id} has relations.");