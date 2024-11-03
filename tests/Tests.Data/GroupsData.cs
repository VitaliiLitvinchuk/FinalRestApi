using Domain.Groups;

namespace Tests.Data;

public static class GroupsData
{
    public static Group MainGroup()
        => Group.New(GroupId.New(), "Main group", "Main group description", DateTime.UtcNow);
}
