using Domain.UsersAssignments;

namespace Domain.Statuses;

public class Status(StatusId id, string name)
{
    public StatusId Id { get; } = id;
    public string Name { get; private set; } = name;

    public ICollection<UserAssignment> UserAssignments { get; } = [];

    public static Status New(StatusId id, string name)
        => new(id, name);

    public void UpdateDetails(string name)
    {
        Name = name;
    }
}
