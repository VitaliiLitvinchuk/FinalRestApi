namespace Domain.Statuses;

public record StatusId(Guid Value)
{
    public static StatusId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
