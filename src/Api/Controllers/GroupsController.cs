using Api.Dtos.Groups;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Groups.Exceptions;
using Domain.Groups;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupsController(IGroupQueries groupQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GroupDto>>> GetAll(CancellationToken cancellationToken)
        {
            var groups = await groupQueries.GetAllAsync(cancellationToken);

            return Ok(groups.Select(GroupDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<GroupDto>> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest();

            GroupId groupId = new(id);

            var group = await groupQueries.GetByIdAsync(groupId, cancellationToken);

            return group.Match<ActionResult<GroupDto>>(
                group => Ok(GroupDto.FromDomainModel(group)),
                () => new GroupNotFoundException(groupId).ToObjectResult()
            );
        }
    }
}
