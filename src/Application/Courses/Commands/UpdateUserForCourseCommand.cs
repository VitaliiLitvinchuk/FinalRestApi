using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Courses.Exceptions;
using Domain.Courses;
using Domain.Users;
using MediatR;

namespace Application.Courses.Commands;

public record UpdateUserForCourseCommand : IRequest<Result<Course, CourseException>>
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
}

public class UpdateUserForCourseCommandHandler(ICourseRepository repository, ICourseQueries queries, IUserQueries userQueries) : IRequestHandler<UpdateUserForCourseCommand, Result<Course, CourseException>>
{
    public async Task<Result<Course, CourseException>> Handle(UpdateUserForCourseCommand request, CancellationToken cancellationToken)
    {
        var id = new CourseId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async course =>
            {
                var userId = new UserId(request.UserId);

                var userOption = await userQueries.GetByIdAsync(userId, cancellationToken);

                return await userOption.Match(
                    async user => await UpdateUser(course, user.Id, cancellationToken),
                    () => Task.FromResult<Result<Course, CourseException>>(new UserForCourseNotFoundException(id, userId))
                );
            },
            () => Task.FromResult<Result<Course, CourseException>>(new CourseNotFoundException(id))
        );
    }

    private async Task<Result<Course, CourseException>> UpdateUser(Course entity, UserId userId, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateUser(userId);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnknownException(entity.Id, exception);
        }
    }
}
