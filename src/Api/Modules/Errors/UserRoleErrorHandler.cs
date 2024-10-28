using Application.UserRoles.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserRoleErrorHandler
{
    public static ObjectResult ToObjectResult(this UserRoleException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            UserRoleUnknownException => StatusCodes.Status400BadRequest,
            UserRoleNotFoundException => StatusCodes.Status404NotFound,
            UserRoleHasRelationsException => StatusCodes.Status409Conflict,
            UserRoleAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled user role exception."),
        }
    };
}
