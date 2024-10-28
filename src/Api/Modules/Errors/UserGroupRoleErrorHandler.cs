using Application.UserGroupRoles.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserGroupRoleErrorHandler
{
    public static ObjectResult ToObjectResult(this UserGroupRoleException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            UserGroupRoleUnknownException => StatusCodes.Status400BadRequest,
            UserGroupRoleNotFoundException => StatusCodes.Status404NotFound,
            UserGroupRoleHasRelationsException => StatusCodes.Status409Conflict,
            UserGroupRoleAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled user group role exception."),
        }
    };
}
