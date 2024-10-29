namespace Api.Dtos.Courses;

public record CourseCreateDto(string Name, string Description, Guid UserId, Guid GroupId);
