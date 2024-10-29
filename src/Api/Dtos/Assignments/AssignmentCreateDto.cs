namespace Api.Dtos.Assignments;

public record AssignmentCreateDto(string Title, string Description, DateTime DueDate, decimal MaxScore, Guid CourseId);
