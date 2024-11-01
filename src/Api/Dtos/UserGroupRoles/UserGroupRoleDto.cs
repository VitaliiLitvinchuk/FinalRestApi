using Domain.UserGroupRoles;

namespace Api.Dtos.UserGroupRoles;

public record UserGroupRoleDto(Guid Id, string Name)
{
    public static UserGroupRoleDto? FromDomainModel(UserGroupRole userGroupRole)
        => userGroupRole is null ? null : new(userGroupRole.Id.Value, userGroupRole.Name);

    public static UserGroupRole ToDomainModel(UserGroupRoleDto userGroupRole)
        => UserGroupRole.New(new(userGroupRole.Id), userGroupRole.Name);
}
