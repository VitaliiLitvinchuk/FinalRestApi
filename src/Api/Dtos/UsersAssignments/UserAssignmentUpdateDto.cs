namespace Api.Dtos.UsersAssignments;

public record UserAssignmentUpdateDto(Guid UserId, Guid AssignmentId, Guid StatusId, DateTime? SubmittedAt = null, decimal? Score = null);
