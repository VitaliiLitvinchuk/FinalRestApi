using Api.Dtos.UsersGroups;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.UsersGroups.Exceptions;
using Domain.Groups;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersGroupsController(IUserGroupQueries userGroupQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserGroupDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userGroups = await userGroupQueries.GetAllAsync(cancellationToken);

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
    }
}
