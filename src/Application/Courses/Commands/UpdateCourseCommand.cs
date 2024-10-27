using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Courses.Exceptions;
using Domain.Courses;
using MediatR;

namespace Application.Courses.Commands;

public record UpdateCourseCommand : IRequest<Result<Course, CourseException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class UpdateCourseCommandHandler(ICourseRepository repository, ICourseQueries queries) : IRequestHandler<UpdateCourseCommand, Result<Course, CourseException>>
{
    public async Task<Result<Course, CourseException>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var id = new CourseId(request.Id);

        var result = await queries.GetByIdAsync(id, cancellationToken);

        return await result.Match(
            async course => await UpdateEntity(course, request.Name, request.Description, cancellationToken),
            () => Task.FromResult<Result<Course, CourseException>>(new CourseNotFoundException(id))
        );
    }

    private async Task<Result<Course, CourseException>> UpdateEntity(Course entity, string name, string description, CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(name, description);

            return await repository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new CourseUnknownException(entity.Id, exception);
        }
    }
}
