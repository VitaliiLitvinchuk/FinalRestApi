using Api.Dtos.Groups;
using Api.Dtos.Users;
using Domain.Courses;

namespace Api.Dtos.Courses;

public record CourseDto(Guid Id, string Name, string Description, DateTime CreatedAt, Guid UserId, UserDto? User, Guid GroupId, GroupDto? Group)
{
    public static CourseDto? FromDomainModel(Course course)
        => course is null ? null : new(course.Id.Value, course.Name, course.Description, course.CreatedAt,
            course.UserId.Value, UserDto.FromDomainModel(course.User!), course.GroupId.Value, GroupDto.FromDomainModel(course.Group!));

    public static Course ToDomainModel(CourseDto courseDto)
        => Course.New(new(courseDto.Id), courseDto.Name, courseDto.Description, new(courseDto.User!.Id), new(courseDto.Group!.Id), courseDto.CreatedAt);
}
