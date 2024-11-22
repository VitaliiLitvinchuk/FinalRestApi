using Api.Dtos.UsersAssignments;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.UserAssignments.Commands;
using Application.UserAssignments.Exceptions;
using Domain.Assignments;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersAssignmentsController(IUserAssignmentQueries userAssignmentQueries, ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserAssignmentDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userAssignments = await userAssignmentQueries.GetAllAsync(cancellationToken, includes: [x => x.User, x => x.Assignment, x => x.Status]);

            return Ok(userAssignments.Select(UserAssignmentDto.FromDomainModel).ToList());
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<UserAssignmentDto>> GetByIds([FromQuery] Guid userId, [FromQuery] Guid assignmentId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty || assignmentId == Guid.Empty)
                return BadRequest();

            UserId userId1 = new(userId);
            AssignmentId assignmentId1 = new(assignmentId);

            var userAssignment = await userAssignmentQueries.GetByAssignmentIdAndUserIdAsync(userId1, assignmentId1, cancellationToken);

            return userAssignment.Match<ActionResult<UserAssignmentDto>>(
                userAssignment => Ok(UserAssignmentDto.FromDomainModel(userAssignment)),
                () => new UserAssignmentNotFoundException(userId1, assignmentId1).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UserAssignmentDto>> Create([FromForm] UserAssignmentCreateDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateUserAssignmentCommand
            {
                UserId = dto.UserId,
                AssignmentId = dto.AssignmentId
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserAssignmentDto>>(
                userAssignment => Ok(UserAssignmentDto.FromDomainModel(userAssignment)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<UserAssignmentDto>> Update([FromForm] UserAssignmentUpdateDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateUserAssignmentCommand
            {
                UserId = dto.UserId,
                AssignmentId = dto.AssignmentId,
                StatusId = dto.StatusId,
                Score = dto.Score,
                SubmittedAt = dto.SubmittedAt
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserAssignmentDto>>(
                userAssignment => Ok(UserAssignmentDto.FromDomainModel(userAssignment)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> Delete([FromQuery] UserAssignmentDeleteDto dto, CancellationToken cancellationToken)
        {
            var input = new DeleteUserAssignmentCommand
            {
                UserId = dto.UserId,
                AssignmentId = dto.AssignmentId
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult>(
                userAssignment => Ok(UserAssignmentDto.FromDomainModel(userAssignment)),
                e => e.ToObjectResult()
            );
        }
    }
}
