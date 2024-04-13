using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Database;
using Fleet.Api.Entities;
using Fleet.Api.Extensions;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Api.Features.Ships.Implementations;

/// <summary>
///     Repository for handling Ship entities in the database.
/// </summary>
public class ShipRepository : IShipRepository
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    ///     Initializes a new instance of the ShipRepository class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public ShipRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Create(Ship ship, CancellationToken ct = default)
    {
        await _db.Ships.AddAsync(ship, ct);
    }


    public Task<Ship> Get(int shipId, bool trackChanges = false, CancellationToken ct = default)
    {
        return _db.Ships
            .TrackChanges(trackChanges)
            .Include(x => x.ShipContainers).ThenInclude(x => x.Container)
            .FirstOrDefaultAsync(x => x.Id == shipId, ct);
    }


    public Task<PaginationResult<Ship>> Get(PaginationParams paginationParams, CancellationToken ct = default)
    {
        return _db.Ships
            .TrackChanges(false)
            .Include(x => x.ShipContainers).ThenInclude(x => x.Container)
            .PaginateAsync(paginationParams, ct);
    }


    public void Remove(Ship ship)
    {
        _db.Ships.Remove(ship);
    }

    public async Task<bool> IsNameUnique(string shipName, CancellationToken ct = default)
    {
        return !await _db.Ships.AnyAsync(x => x.Name == shipName, ct);
    }
}