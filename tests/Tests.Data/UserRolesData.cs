using Domain.UserRoles;

namespace Tests.Data;

public static class UserRolesData
{
    public static UserRole MainUserRole()
        => UserRole.New(UserRoleId.New(), "Admin");
}
