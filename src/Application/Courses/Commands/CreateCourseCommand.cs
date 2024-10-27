using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Courses.Exceptions;
using Domain.Courses;
using Domain.Groups;
using Domain.Users;
using MediatR;

namespace Application.Courses.Commands;

public record CreateCourseCommand : IRequest<Result<Course, CourseException>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required Guid UserId { get; init; }
    public required Guid GroupId { get; init; }
}

public class CreateCourseCommandHandler(ICourseRepository repository, IUserQueries userQueries, IGroupQueries groupQueries) : IRequestHandler<CreateCourseCommand, Result<Course, CourseException>>
{
    public async Task<Result<Course, CourseException>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var id = CourseId.New();

        var userId = new UserId(request.UserId);

        var userOption = await userQueries.GetByIdAsync(userId, cancellationToken);

        return await userOption.Match(
            async user =>
            {
                var groupId = new GroupId(request.GroupId);

                var groupOption = await groupQueries.GetByIdAsync(groupId, cancellationToken);

                return await groupOption.Match(
                    async group => await CreateEntity(Course.New(id, request.Name, request.Description, user.Id, group.Id, DateTime.UtcNow), cancellationToken),
                    () => Task.FromResult<Result<Course, CourseException>>(new GroupForCourseNotFoundException(id, groupId))
                );
            },
            () => Task.FromResult<Result<Course, CourseException>>(new UserForCourseNotFoundException(id, userId))
        );
    }

    private async Task<Result<Course, CourseException>> CreateEntity(Course entity, CancellationToken cancellationToken)
    {
        try
        {
            return await repository.Create(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnknownException(entity.Id, exception);
        }
    }
}
