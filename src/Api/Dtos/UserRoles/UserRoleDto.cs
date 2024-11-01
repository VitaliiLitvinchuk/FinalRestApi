using Api.Dtos.Users;
using Domain.UserRoles;

namespace Api.Dtos.UserRoles;

public record UserRoleDto(Guid Id, string Name)
{
    public static UserRoleDto? FromDomainModel(UserRole userRole)
        => userRole is null ? null : new(userRole.Id.Value, userRole.Name);

    public static UserRole ToDomainModel(UserRoleDto userRole)
        => UserRole.New(new(userRole.Id), userRole.Name);
}
