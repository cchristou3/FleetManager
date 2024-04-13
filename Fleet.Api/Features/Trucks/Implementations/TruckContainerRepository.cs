using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Database;
using Fleet.Api.Entities;
using Fleet.Api.Extensions;
using Fleet.Api.Features.Trucks.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Api.Features.Trucks.Implementations;

/// <summary>
///     Repository for handling Truck Container entities in the database.
/// </summary>
public class TruckContainerRepository : ITruckContainerRepository
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    ///     Initializes a new instance of the TruckContainerRepository class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public TruckContainerRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Create(TruckContainer truckContainer, CancellationToken ct)
    {
        await _db.TruckContainers.AddAsync(truckContainer, ct);
    }

    public async Task<int> CountContainersByTruckId(int truckId, CancellationToken ct)
    {
        return await _db.TruckContainers
            .Where(x => x.TruckId == truckId)
            .CountAsync(ct);
    }

    public Task<TruckContainer> Get(int containerId, bool trackChanges, CancellationToken ct)
    {
        return _db.TruckContainers
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(x => x.ContainerId == containerId, ct);
    }

    public Task<TruckContainer> GetLatestLoaded(int truckId, bool trackChanges, CancellationToken ct)
    {
        return _db.TruckContainers
            .TrackChanges(trackChanges)
            .OrderByDescending(x => x.DateLoaded)
            .FirstOrDefaultAsync(x => x.TruckId == truckId, ct);
    }

    public void Remove(TruckContainer truckContainer)
    {
        _db.TruckContainers.Remove(truckContainer);
    }
}