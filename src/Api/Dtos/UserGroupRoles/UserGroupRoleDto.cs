using Api.Dtos.UsersGroups;
using Domain.UserGroupRoles;

namespace Api.Dtos.UserGroupRoles;

public record UserGroupRoleDto(Guid Id, string Name, List<UserGroupDto> UsersGroups)
{
    public static UserGroupRoleDto FromDomainModel(UserGroupRole userGroupRole)
        => new(userGroupRole.Id.Value, userGroupRole.Name, userGroupRole.UserGroups.Select(UserGroupDto.FromDomainModel).ToList());

    public static UserGroupRole ToDomainModel(UserGroupRoleDto userGroupRole)
        => UserGroupRole.New(new(userGroupRole.Id), userGroupRole.Name);
}
