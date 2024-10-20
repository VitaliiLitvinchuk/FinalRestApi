namespace Domain.Groups;

public record GroupId(Guid Value)
{
    public static GroupId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
