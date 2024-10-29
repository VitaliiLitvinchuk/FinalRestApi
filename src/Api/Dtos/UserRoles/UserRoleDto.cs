using Api.Dtos.Users;
using Domain.UserRoles;

namespace Api.Dtos.UserRoles;

public record UserRoleDto(Guid Id, string Name, List<UserDto> Users)
{
    public static UserRoleDto FromDomainModel(UserRole userRole)
        => new(userRole.Id.Value, userRole.Name, userRole.Users.Select(UserDto.FromDomainModel).ToList());

    public static UserRole ToDomainModel(UserRoleDto userRole)
        => UserRole.New(new(userRole.Id), userRole.Name);
}
