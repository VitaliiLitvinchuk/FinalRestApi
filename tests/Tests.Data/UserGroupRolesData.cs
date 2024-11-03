using Domain.UserGroupRoles;

namespace Tests.Data;

public static class UserGroupRolesData
{
    public static UserGroupRole MainUserGroupRole()
        => UserGroupRole.New(UserGroupRoleId.New(), "Leader");
}
