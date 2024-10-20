using Domain.UsersGroups;

namespace Domain.UserGroupRoles;

public class UserGroupRole(UserGroupRoleId id, string name)
{
    public UserGroupRoleId Id { get; } = id;
    public string Name { get; private set; } = name;

    public ICollection<UserGroup> UserGroups { get; } = [];

    public static UserGroupRole New(UserGroupRoleId id, string name)
        => new(id, name);

    public void UpdateDetails(string name)
    {
        Name = name;
    }
}
