using Api.Dtos.Groups;
using Api.Dtos.UserGroupRoles;
using Api.Dtos.Users;
using Domain.UsersGroups;

namespace Api.Dtos.UsersGroups;

public record UserGroupDto(UserDto? User, GroupDto? Group, UserGroupRoleDto? UserGroupRole, DateTime JoinedAt)
{
    public static UserGroupDto? FromDomainModel(UserGroup userGroup)
        => userGroup is null ? null : new(UserDto.FromDomainModel(userGroup.User!), GroupDto.FromDomainModel(userGroup.Group!),
            UserGroupRoleDto.FromDomainModel(userGroup.UserGroupRole!), userGroup.JoinedAt);

    public static UserGroup ToDomainModel(UserGroupDto userGroupDto)
        => UserGroup.New(new(userGroupDto.User!.Id), new(userGroupDto.Group!.Id),
            new(userGroupDto.UserGroupRole!.Id), userGroupDto.JoinedAt);
}
