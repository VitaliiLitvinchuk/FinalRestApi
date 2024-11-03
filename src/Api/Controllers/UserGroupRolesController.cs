using Api.Dtos.UserGroupRoles;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.UserGroupRoles.Commands;
using Application.UserGroupRoles.Exceptions;
using Domain.UserGroupRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserGroupRolesController(IUserGroupRoleQueries userGroupRoleQueries, ISender sender) : ControllerBase
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

        [HttpPost("[action]")]
        public async Task<ActionResult<UserGroupRoleDto>> Create([FromForm] UserGroupRoleCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateUserGroupRoleCommand
            {
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserGroupRoleDto>>(
                userGroupRole => Ok(UserGroupRoleDto.FromDomainModel(userGroupRole)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<UserGroupRoleDto>> Update([FromForm] UserGroupRoleUpdateDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateUserGroupRoleCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserGroupRoleDto>>(
                userGroupRole => Ok(UserGroupRoleDto.FromDomainModel(userGroupRole)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<UserGroupRoleDto>> Delete([FromQuery] UserGroupRoleDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteUserGroupRoleCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserGroupRoleDto>>(
                userGroupRole => Ok(UserGroupRoleDto.FromDomainModel(userGroupRole)),
                e => e.ToObjectResult()
            );
        }
    }
}
