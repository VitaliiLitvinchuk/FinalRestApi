namespace Api.Dtos.UsersGroups;

public record UserGroupUpdateRoleDto(Guid UserId, Guid GroupId, Guid UserGroupRoleId);
