namespace Domain.UserGroupRoles;

public record UserGroupRoleId(Guid Value)
{
    public static UserGroupRoleId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
