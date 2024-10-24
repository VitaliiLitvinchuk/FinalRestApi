using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Statuses;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class StatusRepository(ApplicationDbContext context) : IStatusRepository, IStatusQueries
{
    private DbSet<Status> Statuses { get; } = context.Statuses;
    public async Task<Status> Create(Status status, CancellationToken cancellation)
    {
        await Statuses.AddAsync(status, cancellation);

        await context.SaveChangesAsync(cancellation);

        return status;
    }

    public async Task<Status> Delete(Status status, CancellationToken cancellation)
    {
        Statuses.Remove(status);

        await context.SaveChangesAsync(cancellation);

        return status;
    }

    public async Task<IEnumerable<Status>> GetAllAsync(CancellationToken cancellation)
    {
        return await Statuses
            .Include(x => x.UserAssignments)
            .AsNoTracking()
            .ToListAsync(cancellation);
    }

    public async Task<Option<Status>> GetByIdAsync(StatusId id, CancellationToken cancellation)
    {
        var status = await Statuses
            .Include(x => x.UserAssignments)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        return status is null ? Option.None<Status>() : Option.Some(status);
    }

    public async Task<Option<Status>> GetByNameAsync(string name, CancellationToken cancellation)
    {
        var status = await Statuses
            .Include(x => x.UserAssignments)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellation);

        return status is null ? Option.None<Status>() : Option.Some(status);
    }

    public async Task<Status> Update(Status status, CancellationToken cancellation)
    {
        Statuses.Update(status);

        await context.SaveChangesAsync(cancellation);

        return status;
    }
}
