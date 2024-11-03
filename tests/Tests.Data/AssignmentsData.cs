using Domain.Assignments;
using Domain.Courses;

namespace Tests.Data;

public static class AssignmentsData
{
    public static Assignment MainAssignment(AssignmentId assignmentId, CourseId courseId)
        => Assignment.New(assignmentId, courseId, "Main assignment", "Main assignment description", DateTime.UtcNow.AddDays(7), 100m, DateTime.UtcNow);
}
