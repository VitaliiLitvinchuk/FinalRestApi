using Api.Dtos.Assignments;
using Api.Modules.Errors;
using Application.Assignments.Exceptions;
using Application.Common.Interfaces.Queries;
using Domain.Assignments;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AssignmentsController(IAssignmentQueries assignmentQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AssignmentDto>>> GetAll(CancellationToken cancellationToken)
        {
            // var assignments = await assignmentQueries.GetAllAsync(cancellationToken, includes: [x => x.Course, x => x.Course.User]);
            var assignments = await assignmentQueries.GetAllAsync(cancellationToken, includes: [x => x.Course]);

            return Ok(assignments.Select(AssignmentDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<AssignmentDto>> GetById([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return BadRequest();

            AssignmentId assignmentId = new(id);

            var assignment = await assignmentQueries.GetByIdAsync(assignmentId, cancellationToken);

            return assignment.Match<ActionResult<AssignmentDto>>(
                assignment => Ok(AssignmentDto.FromDomainModel(assignment)),
                () => new AssignmentNotFoundException(assignmentId).ToObjectResult()
            );
        }
    }
}
