using Domain.Courses;
using Domain.UsersGroups;

namespace Domain.Groups;

public class Group(GroupId id, string name, string description, DateTime createdAt)
{
    public GroupId Id { get; } = id;
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public DateTime CreatedAt { get; } = createdAt;

    public ICollection<UserGroup> UserGroups { get; } = [];
    public ICollection<Course> Courses { get; } = [];

    public static Group New(GroupId id, string name, string description, DateTime createdAt)
        => new(id, name, description, createdAt);

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
