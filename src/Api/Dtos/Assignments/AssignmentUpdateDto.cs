namespace Api.Dtos.Assignments;

public record AssignmentUpdateDto(Guid Id, string Title, string Description, DateTime DueDate, int MaxScore);
