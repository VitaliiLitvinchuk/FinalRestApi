using System;

namespace Domain.Assignments;

public record AssignmentId(Guid Value)
{
    public static AssignmentId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
