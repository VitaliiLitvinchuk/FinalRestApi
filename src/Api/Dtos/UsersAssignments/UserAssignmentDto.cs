using Api.Dtos.Assignments;
using Api.Dtos.Statuses;
using Api.Dtos.Users;
using Domain.UsersAssignments;

namespace Api.Dtos.UsersAssignments;

public record UserAssignmentDto(AssignmentDto Assignment, UserDto User, StatusDto Status, DateTime? SubmittedAt = null, decimal? Score = null)
{
    public static UserAssignmentDto FromDomainModel(UserAssignment userAssignment) =>
        new(AssignmentDto.FromDomainModel(userAssignment.Assignment!), UserDto.FromDomainModel(userAssignment.User!),
            StatusDto.FromDomainModel(userAssignment.Status!), userAssignment.SubmittedAt, userAssignment.Score);

    public static UserAssignment ToDomainModel(UserAssignmentDto userAssignmentDto) =>
        UserAssignment.New(new(userAssignmentDto.Assignment.Id), new(userAssignmentDto.User.Id), new(userAssignmentDto.Status.Id),
            userAssignmentDto.SubmittedAt, userAssignmentDto.Score);
}
