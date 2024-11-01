using Api.Dtos.Users;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Users.Exceptions;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController(IUserQueries userQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await userQueries.GetAllAsync(cancellationToken);

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
    }
}