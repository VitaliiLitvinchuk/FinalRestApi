using Domain.Assignments;
using Domain.Users;

namespace Application.UserAssignments.Exceptions;

public abstract class UserAssignmentException(UserId userId, AssignmentId assignmentId, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public UserId UserId { get; } = userId;
    public AssignmentId AssignmentId { get; } = assignmentId;
}

public class UserAssignmentNotFoundException(UserId userId, AssignmentId assignmentId) : UserAssignmentException(userId, assignmentId, $"User with id: {userId} and assignment with id: {assignmentId} not found.");
public class UserAssignmentAlreadyExistsException(UserId userId, AssignmentId assignmentId) : UserAssignmentException(userId, assignmentId, $"User with id: {userId} and assignment with id: {assignmentId} already exists.");
public class UserAssignmentUnknownException(UserId userId, AssignmentId assignmentId, Exception innerException) : UserAssignmentException(userId, assignmentId, $"User with id: {userId} and assignment with id: {assignmentId} is unknown.", innerException);
public class StatusForUserAssignmentNotFoundException(UserId userId, AssignmentId assignmentId) : UserAssignmentException(userId, assignmentId, $"Status for user with id: {userId} and assignment with id: {assignmentId} not found.");
public class DefaultStatusForUserAssignmentNotFoundException(UserId userId, AssignmentId assignmentId) : UserAssignmentException(userId, assignmentId, $"Default status for user with id: {userId} and assignment with id: {assignmentId} not found.");
public class AssignmentForUserAssignmentNotFoundException(UserId userId, AssignmentId assignmentId) : UserAssignmentException(userId, assignmentId, $"Assignment with id: {assignmentId} not found.");
public class UserForUserAssignmentNotFoundException(UserId userId, AssignmentId assignmentId) : UserAssignmentException(userId, assignmentId, $"User with id: {userId} not found.");