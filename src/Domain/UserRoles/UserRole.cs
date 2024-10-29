using Domain.Users;

namespace Domain.UserRoles;

public class UserRole(UserRoleId id, string name)
{
    public UserRoleId Id { get; } = id;
    public string Name { get; private set; } = name;

    public ICollection<User> Users { get; } = [];

    public static UserRole New(UserRoleId id, string name)
        => new(id, name);

    public void UpdateDetails(string name)
    {
        Name = name;
    }
}
