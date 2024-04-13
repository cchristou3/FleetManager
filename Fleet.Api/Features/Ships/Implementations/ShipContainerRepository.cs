using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Database;
using Fleet.Api.Entities;
using Fleet.Api.Extensions;
using Fleet.Api.Features.Ships.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Api.Features.Ships.Implementations;

/// <summary>
///     Repository for handling Ship Container entities in the database.
/// </summary>
public class ShipContainerRepository : IShipContainerRepository
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    ///     Initializes a new instance of the ShipContainerRepository class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public ShipContainerRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Create(ShipContainer shipContainer, CancellationToken ct)
    {
        await _db.ShipContainers.AddAsync(shipContainer, ct);
    }

    public async Task<int> CountContainersByShipId(int shipId, CancellationToken ct)
    {
        return await _db.ShipContainers
            .Where(x => x.ShipId == shipId)
            .CountAsync(ct);
    }

    public Task<ShipContainer> Get(int containerId, bool trackChanges, CancellationToken ct)
    {
        return _db.ShipContainers
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(x => x.ContainerId == containerId, ct);
    }

    public void Remove(ShipContainer shipContainer)
    {
        _db.ShipContainers.Remove(shipContainer);
    }
}