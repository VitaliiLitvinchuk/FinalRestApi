using Api.Dtos.Courses;
using Api.Dtos.UsersAssignments;
using Domain.Assignments;

namespace Api.Dtos.Assignments;

public record AssignmentDto(Guid Id, string Title, string Description, DateTime DueDate, decimal MaxScore, DateTime CreatedAt,
    List<UserAssignmentDto> UserAssignments, CourseDto Course)
{
    public static AssignmentDto FromDomainModel(Assignment assignment)
        => new(assignment.Id.Value, assignment.Title, assignment.Description, assignment.DueDate, assignment.MaxScore, assignment.CreatedAt,
            assignment.UserAssignments.Select(UserAssignmentDto.FromDomainModel).ToList(), CourseDto.FromDomainModel(assignment.Course!));

    public static Assignment ToDomainModel(AssignmentDto assignmentDto)
        => Assignment.New(new(assignmentDto.Id), new(assignmentDto.Course.Id), assignmentDto.Title, assignmentDto.Description,
            assignmentDto.DueDate, assignmentDto.MaxScore, assignmentDto.CreatedAt);
}
