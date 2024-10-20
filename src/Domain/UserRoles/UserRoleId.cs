namespace Domain.UserRoles;

public record UserRoleId(Guid Value)
{
    public static UserRoleId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
