using Domain.UserRoles;
using Domain.Users;

namespace Tests.Data;

public static class UsersData
{
    public static User MainUser(UserRoleId userRoleId)
        => User.New(UserId.New(), "Main", "User", "5j3pO@example.com", "googleId", "avatarUrl", userRoleId, DateTime.UtcNow);
}
