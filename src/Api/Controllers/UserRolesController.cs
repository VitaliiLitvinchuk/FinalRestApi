using Api.Dtos.UserRoles;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.UserRoles.Exceptions;
using Domain.UserRoles;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserRolesController(IUserRoleQueries userRoleQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserRoleDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userRoles = await userRoleQueries.GetAllAsync(cancellationToken);

            return Ok(userRoles.Select(UserRoleDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<UserRoleDto>> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest();

            UserRoleId userRoleId = new(id);

            var userRole = await userRoleQueries.GetByIdAsync(userRoleId, cancellationToken);

            return userRole.Match<ActionResult<UserRoleDto>>(
                userRole => Ok(UserRoleDto.FromDomainModel(userRole)),
                () => new UserRoleNotFoundException(userRoleId).ToObjectResult()
            );
        }
    }
}
