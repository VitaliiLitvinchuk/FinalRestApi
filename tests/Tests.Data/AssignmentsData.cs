using Domain.Assignments;
using Domain.Courses;

namespace Tests.Data;

public static class AssignmentsData
{
    public static Assignment MainAssignment(CourseId courseId)
        => Assignment.New(AssignmentId.New(), courseId, "Main assignment", "Main assignment description", DateTime.UtcNow.AddDays(7), 100m, DateTime.UtcNow);
}
