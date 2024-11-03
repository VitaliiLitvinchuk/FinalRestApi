using Domain.Courses;
using Domain.UsersAssignments;

namespace Domain.Assignments;

public class Assignment(AssignmentId id, CourseId courseId, string title, string description, DateTime dueDate, decimal maxScore, DateTime createdAt)
{
    public AssignmentId Id { get; } = id;
    public CourseId CourseId { get; private set; } = courseId;
    public string Title { get; private set; } = title;
    public string Description { get; private set; } = description;
    public DateTime DueDate { get; private set; } = dueDate;
    public decimal MaxScore { get; private set; } = maxScore;
    public DateTime CreatedAt { get; } = createdAt;

    public Course? Course { get; private set; }

    public ICollection<UserAssignment> UserAssignments { get; } = [];

    public static Assignment New(AssignmentId id, CourseId courseId, string title, string description, DateTime dueDate, decimal maxScore, DateTime createdAt)
        => new(id, courseId, title, description, dueDate, maxScore, createdAt);

    public void UpdateDetails(string title, string description, DateTime dueDate, decimal maxScore)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        MaxScore = maxScore;
    }

    public void UpdateCourse(CourseId courseId)
    {
        CourseId = courseId;
        Course = null;
    }
}
