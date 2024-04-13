using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Database;
using Fleet.Api.Entities;
using Fleet.Api.Extensions;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Api.Features.Trucks.Implementations;

/// <summary>
///     Repository for handling Truck entities in the database.
/// </summary>
public class TruckRepository : ITruckRepository
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    ///     Initializes a new instance of the TruckRepository class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public TruckRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Create(Truck truck, CancellationToken ct = default)
    {
        await _db.Trucks.AddAsync(truck, ct);
    }


    public Task<Truck> Get(int truckId, bool trackChanges = false, CancellationToken ct = default)
    {
        return _db.Trucks
            .TrackChanges(trackChanges)
            .Include(x => x.TruckContainers).ThenInclude(x => x.Container)
            .FirstOrDefaultAsync(x => x.Id == truckId, ct);
    }


    public Task<PaginationResult<Truck>> Get(PaginationParams paginationParams, CancellationToken ct = default)
    {
        return _db.Trucks
            .TrackChanges(false)
            .Include(x => x.TruckContainers).ThenInclude(x => x.Container)
            .PaginateAsync(paginationParams, ct);
    }


    public void Remove(Truck truck)
    {
        _db.Trucks.Remove(truck);
    }

    public async Task<bool> IsNameUnique(string truckName, CancellationToken ct = default)
    {
        return !await _db.Trucks.AnyAsync(x => x.Name == truckName, ct);
    }
}