using Domain.Courses;
using Domain.Groups;
using Domain.Users;

namespace Tests.Data;

public static class CoursesData
{
    public static Course MainCourse(UserId userId, GroupId groupId)
        => Course.New(CourseId.New(), "Main course", "Main course description", userId, groupId, DateTime.UtcNow);
}
