using Application.UsersGroups.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserGroupErrorHandler
{
    public static ObjectResult ToObjectResult(this UserGroupException exception) => new(exception.Message)
    {
        StatusCode = exception switch
        {
            UserGroupUnknownException => StatusCodes.Status400BadRequest,
            UserGroupNotFoundException => StatusCodes.Status404NotFound,
            RoleForUserGroupNotFoundException => StatusCodes.Status404NotFound,
            UserForUserGroupNotFoundException => StatusCodes.Status404NotFound,
            GroupForUserGroupNotFoundException => StatusCodes.Status404NotFound,
            UserGroupDefaultRoleNotFoundException => StatusCodes.Status404NotFound,
            UserGroupAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled user group exception."),
        }
    };
}
