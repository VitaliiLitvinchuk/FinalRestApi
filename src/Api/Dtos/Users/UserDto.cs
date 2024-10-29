using Api.Dtos.Courses;
using Api.Dtos.UserRoles;
using Api.Dtos.UsersAssignments;
using Api.Dtos.UsersGroups;
using Domain.Users;

namespace Api.Dtos.Users;

public record UserDto(Guid Id, string FirstName, string LastName, string Email, string GoogleId, string AvatarUrl,
    UserRoleDto UserRole, List<CourseDto> Courses, List<UserGroupDto> UserGroups, List<UserAssignmentDto> UserAssignments)
{
    public static UserDto FromDomainModel(User user)
        => new(user.Id.Value, user.FirstName, user.LastName, user.Email, user.GoogleId, user.AvatarUrl,
            UserRoleDto.FromDomainModel(user.UserRole!), user.Courses.Select(CourseDto.FromDomainModel).ToList(),
            user.UserGroups.Select(UserGroupDto.FromDomainModel).ToList(),
            user.UserAssignments.Select(UserAssignmentDto.FromDomainModel).ToList());

    public static User ToDomainModel(UserDto userDto)
        => User.New(new(userDto.Id), userDto.FirstName, userDto.LastName, userDto.Email, userDto.GoogleId,
        userDto.AvatarUrl, new(userDto.UserRole.Id), DateTime.UtcNow);
}
