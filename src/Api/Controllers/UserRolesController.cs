using Api.Dtos.UserRoles;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.UserRoles.Commands;
using Application.UserRoles.Exceptions;
using Domain.UserRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserRolesController(IUserRoleQueries userRoleQueries, ISender sender) : ControllerBase
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

        [HttpPost("[action]")]
        public async Task<ActionResult<UserRoleDto>> Create([FromForm] UserRoleCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateUserRoleCommand
            {
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserRoleDto>>(
                userRole => Ok(UserRoleDto.FromDomainModel(userRole)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<UserRoleDto>> Update([FromForm] UserRoleUpdateDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateUserRoleCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserRoleDto>>(
                userRole => Ok(UserRoleDto.FromDomainModel(userRole)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> Delete([FromForm] UserRoleDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteUserRoleCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult>(
                userRole => Ok(UserRoleDto.FromDomainModel(userRole)),
                e => e.ToObjectResult()
            );
        }
    }
}
