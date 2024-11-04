using Domain.UserRoles;
using Domain.Users;

namespace Tests.Data;

public static class UsersData
{
    public static User MainUser(UserRoleId userRoleId)
        => User.New(UserId.New(), "Main", "User", "random@example.com", "googleId", "avatarUrl", userRoleId, DateTime.UtcNow);
}
