using Api.Dtos.Statuses;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Statuses.Commands;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusesController(IStatusQueries statusQueries, ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetStatuses(CancellationToken cancellationToken)
        {
            var statuses = await statusQueries.GetAllAsync(cancellationToken);

            return Ok(statuses.Select(StatusDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<StatusDto>> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest();

            StatusId statusId = new(id);

            var status = await statusQueries.GetByIdAsync(statusId, cancellationToken);

            return status.Match<ActionResult<StatusDto>>(
                status => Ok(StatusDto.FromDomainModel(status)),
                () => new StatusNotFoundException(statusId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<StatusDto>> Create([FromForm] StatusCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateStatusCommand
            {
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<StatusDto>>(
                status => Ok(StatusDto.FromDomainModel(status)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<StatusDto>> Update([FromForm] StatusUpdateDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateStatusCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<StatusDto>>(
                status => Ok(StatusDto.FromDomainModel(status)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<StatusDto>> Delete([FromQuery] StatusDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteStatusCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<StatusDto>>(
                status => Ok(StatusDto.FromDomainModel(status)),
                e => e.ToObjectResult()
            );
        }
    }
}
