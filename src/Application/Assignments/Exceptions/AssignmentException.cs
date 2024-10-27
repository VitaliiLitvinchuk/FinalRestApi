using Domain.Assignments;
using Domain.Courses;

namespace Application.Assignments.Exceptions;

public class AssignmentException(AssignmentId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public AssignmentId Id { get; } = id;
}

public class AssignmentNotFoundException(AssignmentId id) : AssignmentException(id, $"Assignment with id: {id} not found.");
public class AssignmentUnknownException(AssignmentId id, Exception innerException) : AssignmentException(id, $"Assignment with id: {id} is unknown.", innerException);
public class AssignmentHasRelationsException(AssignmentId id) : AssignmentException(id, $"Assignment with id: {id} has relations.");
public class CourseForAssignmentNotFoundException(AssignmentId id, CourseId courseId) : AssignmentException(id, $"Course with id: {courseId} for assignment with id: {id} not found.");