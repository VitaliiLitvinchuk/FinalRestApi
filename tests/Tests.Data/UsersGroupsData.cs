using Domain.Groups;
using Domain.UserGroupRoles;
using Domain.Users;
using Domain.UsersGroups;

namespace Tests.Data;

public static class UsersGroupsData
{
    public static UserGroup MainUserGroup(UserId userId, GroupId groupId, UserGroupRoleId userGroupRoleId)
        => UserGroup.New(userId, groupId, userGroupRoleId, DateTime.UtcNow);
}
