using Api.Dtos.Courses;
using Domain.Assignments;

namespace Api.Dtos.Assignments;

public record AssignmentDto(Guid Id, string Title, string Description, DateTime DueDate, decimal MaxScore, DateTime CreatedAt, CourseDto? Course)
{
    public static AssignmentDto? FromDomainModel(Assignment assignment)
        => assignment is null ? null : new(assignment.Id.Value, assignment.Title, assignment.Description, assignment.DueDate, assignment.MaxScore, assignment.CreatedAt,
            CourseDto.FromDomainModel(assignment.Course!));

    public static Assignment ToDomainModel(AssignmentDto assignmentDto)
        => Assignment.New(new(assignmentDto.Id), new(assignmentDto.Course!.Id), assignmentDto.Title, assignmentDto.Description,
            assignmentDto.DueDate, assignmentDto.MaxScore, assignmentDto.CreatedAt);
}
