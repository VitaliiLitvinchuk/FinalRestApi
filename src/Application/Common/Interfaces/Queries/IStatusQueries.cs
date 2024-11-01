using System.Linq.Expressions;
using Domain.Statuses;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IStatusQueries
{
    public Task<Option<Status>> GetByIdAsync(StatusId id, CancellationToken cancellation);
    public Task<IEnumerable<Status>> GetAllAsync(CancellationToken cancellation,
        Expression<Func<Status, bool>>? filter = null,
        Func<IQueryable<Status>, IOrderedQueryable<Status>>? orderBy = null,
        params Expression<Func<Status, object?>>[] includes);
    public Task<Option<Status>> GetByNameAsync(string name, CancellationToken cancellation);
}
