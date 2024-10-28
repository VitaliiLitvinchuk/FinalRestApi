using Application.Courses.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class CourseErrorHandler
{
    public static ObjectResult ToObjectResult(this CourseException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            CourseUnknownException => StatusCodes.Status400BadRequest,
            GroupForCourseNotFoundException => StatusCodes.Status404NotFound,
            UserForCourseNotFoundException => StatusCodes.Status404NotFound,
            CourseNotFoundException => StatusCodes.Status404NotFound,
            CourseHasRelationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled course exception."),
        }
    };
}
