using Api.Dtos.Statuses;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Statuses.Exceptions;
using Domain.Statuses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusesController(IStatusQueries statusQueries) : ControllerBase
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
    }
}
