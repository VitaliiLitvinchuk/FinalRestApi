using Domain.Courses;
using Domain.UserRoles;
using Domain.UsersAssignments;
using Domain.UsersGroups;

namespace Domain.Users;

public class User(UserId id, string firstName, string lastName, string email, string googleId, string avatarUrl, UserRoleId userRoleId, DateTime createdAt)
{
    public UserId Id { get; } = id;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string Email { get; private set; } = email;
    public string GoogleId { get; private set; } = googleId;
    public string AvatarUrl { get; private set; } = avatarUrl;
    public UserRoleId UserRoleId { get; } = userRoleId;
    public DateTime CreatedAt { get; } = createdAt;

    public UserRole? UserRole { get; }

    public ICollection<Course> Courses { get; } = [];
    public ICollection<UserGroup> UserGroups { get; } = [];
    public ICollection<UserAssignment> UserAssignments { get; } = [];

    public static User New(UserId id, string firstName, string lastName, string email, string googleId, string avatarUrl, UserRoleId userRoleId, DateTime createdAt)
        => new(id, firstName, lastName, email, googleId, avatarUrl, userRoleId, createdAt);

    public void UpdateDetail(string firstName, string lastName, string avatarUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        AvatarUrl = avatarUrl;
    }
}
