using Api.Dtos.UsersGroups;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.UsersGroups.Commands;
using Application.UsersGroups.Exceptions;
using Domain.Groups;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersGroupsController(IUserGroupQueries userGroupQueries, ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserGroupDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userGroups = await userGroupQueries.GetAllAsync(cancellationToken, includes: [x => x.User, x => x.Group, x => x.UserGroupRole]);

            return Ok(userGroups.Select(UserGroupDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<UserGroupDto>> GetByIds([FromQuery] Guid userId, [FromQuery] Guid groupId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty || groupId == Guid.Empty)
                return BadRequest();

            UserId userId1 = new(userId);
            GroupId groupId1 = new(groupId);

            var userGroup = await userGroupQueries.GetByUserIdAndGroupIdAsync(userId1, groupId1, cancellationToken);

            return userGroup.Match<ActionResult<UserGroupDto>>(
                userGroup => Ok(UserGroupDto.FromDomainModel(userGroup)),
                () => new UserGroupNotFoundException(userId1, groupId1).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UserGroupDto>> Create([FromForm] UserGroupCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateUserGroupCommand
            {
                UserId = dto.UserId,
                GroupId = dto.GroupId
            };

            var userGroup = await sender.Send(input, cancellationToken);

            return userGroup.Match<ActionResult<UserGroupDto>>(
                userGroup => Ok(UserGroupDto.FromDomainModel(userGroup)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<UserGroupDto>> UpdateUserRole([FromForm] UserGroupUpdateRoleDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateRoleForUserGroupCommand
            {
                UserId = dto.UserId,
                GroupId = dto.GroupId,
                UserGroupRoleId = dto.UserGroupRoleId
            };

            var userGroup = await sender.Send(input, cancellationToken);

            return userGroup.Match<ActionResult<UserGroupDto>>(
                userGroup => Ok(UserGroupDto.FromDomainModel(userGroup)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<UserGroupDto>> Delete([FromQuery] UserGroupDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteUserGroupCommand
            {
                UserId = dto.UserId,
                GroupId = dto.GroupId
            };

            var userGroup = await sender.Send(input, cancellationToken);

            return userGroup.Match<ActionResult<UserGroupDto>>(
                userGroup => Ok(UserGroupDto.FromDomainModel(userGroup)),
                e => e.ToObjectResult()
            );
        }
    }
}
