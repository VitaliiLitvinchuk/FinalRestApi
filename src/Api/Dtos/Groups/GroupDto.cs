using Domain.Groups;

namespace Api.Dtos.Groups;

public record GroupDto(Guid Id, string Name, string Description, DateTime CreatedAt)
{
    public static GroupDto? FromDomainModel(Group group)
        => group is null ? null : new(group.Id.Value, group.Name, group.Description, group.CreatedAt);

    public static Group ToDomainModel(GroupDto groupDto)
        => Group.New(new(groupDto.Id), groupDto.Name, groupDto.Description, groupDto.CreatedAt);
}
