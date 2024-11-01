using Domain.Statuses;

namespace Api.Dtos.Statuses;

public record StatusDto(Guid Id, string Name)
{
    public static StatusDto? FromDomainModel(Status status)
        => status is null ? null
        : new(status.Id.Value, status.Name);

    public static Status ToDomainModel(StatusDto statusDto)
        => Status.New(new(statusDto.Id), statusDto.Name);
}
