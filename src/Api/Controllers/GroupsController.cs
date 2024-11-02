using Api.Dtos.Groups;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Groups.Commands;
using Application.Groups.Exceptions;
using Domain.Groups;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupsController(IGroupQueries groupQueries, ISender sender) : ControllerBase
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

        [HttpPost("[action]")]
        public async Task<ActionResult<GroupDto>> Create([FromForm] GroupCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateGroupCommand
            {
                Name = dto.Name,
                Description = dto.Description,
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<GroupDto>>(
                group => Ok(GroupDto.FromDomainModel(group)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<GroupDto>> Update([FromForm] GroupUpdateDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateGroupCommand
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<GroupDto>>(
                group => Ok(GroupDto.FromDomainModel(group)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<GroupDto>> Delete([FromQuery] GroupDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteGroupCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<GroupDto>>(
                group => Ok(GroupDto.FromDomainModel(group)),
                e => e.ToObjectResult()
            );
        }
    }
}
