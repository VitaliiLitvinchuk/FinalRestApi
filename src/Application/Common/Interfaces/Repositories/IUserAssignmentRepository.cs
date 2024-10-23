using Domain.UsersAssignments;

namespace Application.Common.Interfaces.Repositories;

public interface IUserAssignmentRepository
{
    public Task<UserAssignment> Create(UserAssignment userAssignment, CancellationToken cancellation);
    public Task<UserAssignment> Update(UserAssignment userAssignment, CancellationToken cancellation);
    public Task<UserAssignment> Delete(UserAssignment userAssignment, CancellationToken cancellation);
}
