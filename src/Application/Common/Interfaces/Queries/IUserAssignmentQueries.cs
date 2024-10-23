using Domain.Assignments;
using Domain.Statuses;
using Domain.Users;
using Domain.UsersAssignments;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserAssignmentQueries
{
    public Task<Option<UserAssignment>> GetByAssignmentIdAndUserIdAsync(UserId userId, AssignmentId assignmentId, CancellationToken cancellation);
    public Task<IEnumerable<UserAssignment>> GetByUserIdAsync(UserId userId, CancellationToken cancellation);
    public Task<IEnumerable<UserAssignment>> GetByAssignmentIdAsync(AssignmentId assignmentId, CancellationToken cancellation);
    public Task<IEnumerable<UserAssignment>> GetByUserIdAndStatusIdAsync(UserId userId, StatusId statusId, CancellationToken cancellation);
    public Task<IEnumerable<UserAssignment>> GetByAssignmentIdAndStatusIdAsync(AssignmentId assignmentId, StatusId statusId, CancellationToken cancellation);
    public Task<IEnumerable<UserAssignment>> GetAllAsync(CancellationToken cancellation);
}
