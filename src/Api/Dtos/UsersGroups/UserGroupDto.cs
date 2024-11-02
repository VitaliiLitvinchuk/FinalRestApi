using Api.Dtos.Groups;
using Api.Dtos.UserGroupRoles;
using Api.Dtos.Users;
using Domain.UsersGroups;

namespace Api.Dtos.UsersGroups;

public record UserGroupDto(Guid UserId, UserDto? User, Guid GroupId, GroupDto? Group, Guid UserGroupRoleId, UserGroupRoleDto? UserGroupRole, DateTime JoinedAt)
{
    public static UserGroupDto? FromDomainModel(UserGroup userGroup)
        => userGroup is null ? null
            : new(userGroup.UserId.Value, UserDto.FromDomainModel(userGroup.User!),
            userGroup.GroupId.Value, GroupDto.FromDomainModel(userGroup.Group!),
            userGroup.UserGroupRoleId.Value, UserGroupRoleDto.FromDomainModel(userGroup.UserGroupRole!), userGroup.JoinedAt);

    public static UserGroup ToDomainModel(UserGroupDto userGroupDto)
        => UserGroup.New(new(userGroupDto.User!.Id), new(userGroupDto.Group!.Id),
            new(userGroupDto.UserGroupRole!.Id), userGroupDto.JoinedAt);
}
