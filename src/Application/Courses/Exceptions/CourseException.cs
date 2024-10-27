using Domain.Courses;
using Domain.Groups;
using Domain.Users;

namespace Application.Courses.Exceptions;

public abstract class CourseException(CourseId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public CourseId Id { get; } = id;
}

public class CourseNotFoundException(CourseId id) : CourseException(id, $"Course with id: {id} not found.");
public class CourseHasRelationsException(CourseId id) : CourseException(id, $"Course with id: {id} has relations.");
public class CourseUnknownException(CourseId id, Exception innerException) : CourseException(id, $"Course with id: {id} is unknown.", innerException);
public class GroupForCourseNotFoundException(CourseId id, GroupId groupId) : CourseException(id, $"Group with id: {groupId} for course with id: {id} not found.");
public class UserForCourseNotFoundException(CourseId id, UserId userId) : CourseException(id, $"User with id: {userId} for course with id: {id} not found.");