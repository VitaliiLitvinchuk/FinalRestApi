using Application.Groups.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class GroupErrorHandler
{
    public static ObjectResult ToObjectResult(this GroupException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            GroupUnknownException => StatusCodes.Status400BadRequest,
            GroupNotFoundException => StatusCodes.Status404NotFound,
            GroupHasRelationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled group exception."),
        }
    };
}
