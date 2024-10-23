using Domain.Assignments;

namespace Application.Common.Interfaces.Repositories;

public interface IAssignmentRepository
{
    public Task<Assignment> Create(Assignment assignment, CancellationToken cancellation);
    public Task<Assignment> Update(Assignment assignment, CancellationToken cancellation);
    public Task<Assignment> Delete(Assignment assignment, CancellationToken cancellation);
}
