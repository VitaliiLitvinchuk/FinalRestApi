using Domain.Assignments;
using Domain.Groups;
using Domain.Users;

namespace Domain.Courses;

public class Course(CourseId id, string name, string description, UserId userId, GroupId groupId, DateTime createdAt)
{
    public CourseId Id { get; } = id;
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public UserId UserId { get; private set; } = userId;
    public GroupId GroupId { get; private set; } = groupId;
    public DateTime CreatedAt { get; } = createdAt;

    public User? User { get; private set; }
    public Group? Group { get; private set; }

    public ICollection<Assignment> Assignments { get; } = [];

    public static Course New(CourseId id, string name, string description, UserId userId, GroupId groupId, DateTime createdAt)
        => new(id, name, description, userId, groupId, createdAt);

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void UpdateGroup(GroupId groupId)
    {
        GroupId = groupId;
        Group = null;
    }

    public void UpdateUser(UserId userId)
    {
        UserId = userId;
        User = null;
    }
}
