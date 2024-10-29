using Api.Dtos.Courses;
using Api.Dtos.UsersGroups;
using Domain.Groups;

namespace Api.Dtos.Groups;

public record GroupDto(Guid Id, string Name, string Description, DateTime CreatedAt, List<UserGroupDto> UserAssignments, List<CourseDto> Courses)
{
    public static GroupDto FromDomainModel(Group group)
        => new(group.Id.Value, group.Name, group.Description, group.CreatedAt,
            group.UserGroups.Select(UserGroupDto.FromDomainModel).ToList(), group.Courses.Select(CourseDto.FromDomainModel).ToList());

    public static Group ToDomainModel(GroupDto groupDto)
        => Group.New(new(groupDto.Id), groupDto.Name, groupDto.Description, groupDto.CreatedAt);
}
