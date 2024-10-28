using Application.Assignments.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class AssignmentErrorHandler
{
    public static ObjectResult ToObjectResult(this AssignmentException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            AssignmentUnknownException => StatusCodes.Status400BadRequest,
            AssignmentNotFoundException => StatusCodes.Status404NotFound,
            CourseForAssignmentNotFoundException => StatusCodes.Status404NotFound,
            AssignmentHasRelationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled assignment exception.")
        }
    };
}
