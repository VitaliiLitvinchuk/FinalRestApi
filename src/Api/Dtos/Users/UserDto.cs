using Api.Dtos.UserRoles;
using Domain.Users;

namespace Api.Dtos.Users;

public record UserDto(Guid Id, string FirstName, string LastName, string Email, string GoogleId, string AvatarUrl,
    Guid UserRoleId, UserRoleDto? UserRole)
{
    public static UserDto? FromDomainModel(User user)
        => user is null ? null : new(user.Id.Value, user.FirstName, user.LastName, user.Email, user.GoogleId, user.AvatarUrl,
            user.Id.Value, UserRoleDto.FromDomainModel(user.UserRole!));

    public static User ToDomainModel(UserDto userDto)
        => User.New(new(userDto.Id), userDto.FirstName, userDto.LastName, userDto.Email, userDto.GoogleId,
        userDto.AvatarUrl, new(userDto.UserRole!.Id), DateTime.UtcNow);
}
