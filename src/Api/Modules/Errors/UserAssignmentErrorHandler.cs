using Application.UserAssignments.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserAssignmentErrorHandler
{
    public static ObjectResult ToObjectResult(this UserAssignmentException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            UserAssignmentUnknownException => StatusCodes.Status400BadRequest,
            UserAssignmentNotFoundException => StatusCodes.Status404NotFound,
            UserForUserAssignmentNotFoundException => StatusCodes.Status404NotFound,
            StatusForUserAssignmentNotFoundException => StatusCodes.Status404NotFound,
            AssignmentForUserAssignmentNotFoundException => StatusCodes.Status404NotFound,
            DefaultStatusForUserAssignmentNotFoundException => StatusCodes.Status404NotFound,
            UserAssignmentAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled user assignment exception."),
        }
    };
}
