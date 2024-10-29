using Api.Dtos.UsersAssignments;
using Domain.Statuses;

namespace Api.Dtos.Statuses;

public record StatusDto(Guid Id, string Name, List<UserAssignmentDto> UsersAssignments)
{
    public static StatusDto FromDomainModel(Status status)
        => new(status.Id.Value, status.Name, status.UserAssignments.Select(UserAssignmentDto.FromDomainModel).ToList());

    public static Status ToDomainModel(StatusDto statusDto)
        => Status.New(new(statusDto.Id), statusDto.Name);
}
