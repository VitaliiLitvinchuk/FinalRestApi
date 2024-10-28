using Application.Statuses.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class StatusErrorHandler
{
    public static ObjectResult ToObjectResult(this StatusException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            StatusUnknownException => StatusCodes.Status400BadRequest,
            StatusNotFoundException => StatusCodes.Status404NotFound,
            StatusAlreadyExistsException => StatusCodes.Status409Conflict,
            StatusHasRelationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled status exception."),
        }
    };
}
