using Api.Dtos.Assignments;
using Api.Dtos.Statuses;
using Api.Dtos.Users;
using Domain.UsersAssignments;

namespace Api.Dtos.UsersAssignments;

public record UserAssignmentDto(Guid AssignmentId, AssignmentDto? Assignment, Guid UserId, UserDto? User, Guid StatusId, StatusDto? Status, DateTime? SubmittedAt = null, decimal? Score = null)
{
    public static UserAssignmentDto? FromDomainModel(UserAssignment userAssignment)
        => userAssignment is null ? null
            : new(userAssignment.AssignmentId.Value, AssignmentDto.FromDomainModel(userAssignment.Assignment!),
            userAssignment.UserId.Value, UserDto.FromDomainModel(userAssignment.User!),
            userAssignment.StatusId.Value, StatusDto.FromDomainModel(userAssignment.Status!),
            userAssignment.SubmittedAt, userAssignment.Score);

    public static UserAssignment ToDomainModel(UserAssignmentDto userAssignmentDto) =>
        UserAssignment.New(new(userAssignmentDto.Assignment!.Id), new(userAssignmentDto.User!.Id), new(userAssignmentDto.Status!.Id),
            userAssignmentDto.SubmittedAt, userAssignmentDto.Score);
}
