using Domain.Groups;
using Domain.UserGroupRoles;
using Domain.Users;

namespace Domain.UsersGroups;

public class UserGroup(UserId userId, GroupId groupId, UserGroupRoleId userGroupRoleId, DateTime joinedAt)
{
    public UserId UserId { get; } = userId;
    public GroupId GroupId { get; } = groupId;
    public UserGroupRoleId UserGroupRoleId { get; private set; } = userGroupRoleId;
    public DateTime JoinedAt { get; } = joinedAt;

    public User? User { get; }
    public Group? Group { get; }
    public UserGroupRole? UserGroupRole { get; }

    public static UserGroup New(UserId userId, GroupId groupId, UserGroupRoleId userGroupRoleId, DateTime joinedAt)
        => new(userId, groupId, userGroupRoleId, joinedAt);

    public void UpdateDetails(UserGroupRoleId userGroupRoleId)
    {
        UserGroupRoleId = userGroupRoleId;
    }
}