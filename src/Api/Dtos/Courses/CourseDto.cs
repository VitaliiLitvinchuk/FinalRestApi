using Api.Dtos.Assignments;
using Api.Dtos.Groups;
using Api.Dtos.Users;
using Domain.Courses;

namespace Api.Dtos.Courses;

public record CourseDto(Guid Id, string Name, string Description, DateTime CreatedAt, List<AssignmentDto> Assignments, UserDto User, GroupDto Group)
{
    public static CourseDto FromDomainModel(Course course)
        => new(course.Id.Value, course.Name, course.Description, course.CreatedAt,
            course.Assignments.Select(AssignmentDto.FromDomainModel).ToList(),
            UserDto.FromDomainModel(course.User!), GroupDto.FromDomainModel(course.Group!));

    public static Course ToDomainModel(CourseDto courseDto)
        => Course.New(new(courseDto.Id), courseDto.Name, courseDto.Description, new(courseDto.User.Id), new(courseDto.Group.Id), courseDto.CreatedAt);
}
