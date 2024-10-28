using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserErrorHandler
{
    public static ObjectResult ToObjectResult(this UserException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            UserUnknownException => StatusCodes.Status400BadRequest,
            UserNotFoundException => StatusCodes.Status404NotFound,
            RoleForUserNotFoundException => StatusCodes.Status404NotFound,
            UserDefaultRoleNotFoundException => StatusCodes.Status404NotFound,
            UserAlreadyExistsException => StatusCodes.Status409Conflict,
            UserHasRelationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled user exception."),
        }
    };
}
