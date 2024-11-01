using Api.Dtos.UserGroupRoles;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.UserGroupRoles.Exceptions;
using Domain.UserGroupRoles;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserGroupRolesController(IUserGroupRoleQueries userGroupRoleQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserGroupRoleDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userGroupRoles = await userGroupRoleQueries.GetAllAsync(cancellationToken);

            return Ok(userGroupRoles.Select(UserGroupRoleDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<UserGroupRoleDto>> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest();

            UserGroupRoleId userGroupRoleId = new(id);

            var userGroupRole = await userGroupRoleQueries.GetByIdAsync(userGroupRoleId, cancellationToken);

            return userGroupRole.Match<ActionResult<UserGroupRoleDto>>(
                userGroupRole => Ok(UserGroupRoleDto.FromDomainModel(userGroupRole)),
                () => new UserGroupRoleNotFoundException(userGroupRoleId).ToObjectResult()
            );
        }
    }
}
