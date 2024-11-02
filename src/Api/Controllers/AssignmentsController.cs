using Api.Dtos.Assignments;
using Api.Modules.Errors;
using Application.Assignments.Commands;
using Application.Assignments.Exceptions;
using Application.Common.Interfaces.Queries;
using Domain.Assignments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AssignmentsController(IAssignmentQueries assignmentQueries, ISender sender) : ControllerBase
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

        [HttpPost("[action]")]
        public async Task<ActionResult<AssignmentDto>> Create([FromForm] AssignmentCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateAssignmentCommand
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                MaxScore = dto.MaxScore
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<AssignmentDto>>(
                assignment => Ok(AssignmentDto.FromDomainModel(assignment)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<AssignmentDto>> Update([FromForm] AssignmentUpdateDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateAssignmentCommand
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                MaxScore = dto.MaxScore
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<AssignmentDto>>(
                assignment => Ok(AssignmentDto.FromDomainModel(assignment)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<AssignmentDto>> UpdateCourse([FromForm] AssignmentUpdateCourseDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateCourseForAssignmentCommand
            {
                Id = dto.Id,
                CourseId = dto.CourseId
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<AssignmentDto>>(
                assignment => Ok(AssignmentDto.FromDomainModel(assignment)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<AssignmentDto>> Delete([FromForm] AssignmentDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteAssignmentCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<AssignmentDto>>(
                assignment => Ok(AssignmentDto.FromDomainModel(assignment)),
                e => e.ToObjectResult()
            );
        }
    }
}
