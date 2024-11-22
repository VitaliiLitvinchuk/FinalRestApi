using Api.Dtos.Users;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController(IUserQueries userQueries, ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await userQueries.GetAllAsync(cancellationToken, includes: [x => x.UserRole]);

            return Ok(users.Select(UserDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<UserDto>> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest();

            UserId userId = new(id);

            var user = await userQueries.GetByIdAsync(userId, cancellationToken);

            return user.Match<ActionResult<UserDto>>(
                user => Ok(UserDto.FromDomainModel(user)),
                () => new UserNotFoundException(userId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UserDto>> Create([FromForm] UserCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateUserCommand
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                GoogleId = dto.GoogleId,
                AvatarUrl = dto.AvatarUrl
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserDto>>(
                user => Ok(UserDto.FromDomainModel(user)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<UserDto>> Update([FromForm] UserUpdateDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateUserCommand
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                AvatarUrl = dto.AvatarUrl
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserDto>>(
                user => Ok(UserDto.FromDomainModel(user)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<UserDto>> UpdateRole([FromForm] UserUpdateRoleDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateRoleForUserCommand
            {
                Id = dto.Id,
                UserRoleId = dto.UserRoleId
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserDto>>(
                user => Ok(UserDto.FromDomainModel(user)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> Delete([FromQuery] UserDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteUserCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult>(
                user => Ok(UserDto.FromDomainModel(user)),
                e => e.ToObjectResult()
            );
        }
    }
}