using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Courses.Exceptions;
using Domain.Courses;
using Domain.Groups;
using MediatR;

namespace Application.Courses.Commands;

public record UpdateGroupForCourseCommand : IRequest<Result<Course, CourseException>>
{
    public required Guid Id { get; init; }
    public required Guid GroupId { get; init; }
}

public class UpdateGroupForCourseCommandHandler(ICourseRepository repository, ICourseQueries queries, IGroupQueries groupQueries) : IRequestHandler<UpdateGroupForCourseCommand, Result<Course, CourseException>>
{
    public async Task<Result<Course, CourseException>> Handle(UpdateGroupForCourseCommand request, CancellationToken cancellationToken)
    {
        var id = new CourseId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async course =>
            {
                var groupId = new GroupId(request.GroupId);

                var groupOption = await groupQueries.GetByIdAsync(groupId, cancellationToken);

                return await groupOption.Match(
                    async group => await UpdateEntity(course, group.Id, cancellationToken),
                    () => Task.FromResult<Result<Course, CourseException>>(new GroupForCourseNotFoundException(id, groupId))
                );
            },
            () => Task.FromResult<Result<Course, CourseException>>(new CourseNotFoundException(id))
        );
    }

    private async Task<Result<Course, CourseException>> UpdateEntity(Course entity, GroupId groupId, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateGroup(groupId);
            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnknownException(entity.Id, exception);
        }
    }
}
