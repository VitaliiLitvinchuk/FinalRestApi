using Domain.Assignments;
using Domain.Groups;
using Domain.Users;

namespace Domain.Courses;

public class Course(CourseId id, string name, string description, UserId userId, GroupId groupId, DateTime createdAt)
{
    public CourseId Id { get; } = id;
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public UserId UserId { get; } = userId;
    public GroupId GroupId { get; } = groupId;
    public DateTime CreatedAt { get; } = createdAt;

    public User? User { get; }
    public Group? Group { get; }
    public ICollection<Assignment> Assignments { get; } = [];

    public static Course New(CourseId id, string name, string description, UserId userId, GroupId groupId, DateTime createdAt)
        => new(id, name, description, userId, groupId, createdAt);

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

