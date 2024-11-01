using Api.Dtos.UsersAssignments;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.UserAssignments.Exceptions;
using Domain.Assignments;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersAssignmentsController(IUserAssignmentQueries userAssignmentQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserAssignmentDto>>> GetAll(CancellationToken cancellationToken)
        {
            var userAssignments = await userAssignmentQueries.GetAllAsync(cancellationToken);

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
    }
}
