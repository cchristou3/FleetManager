using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Database;
using Fleet.Api.Entities;
using Fleet.Api.Extensions;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Api.Features.Containers.Implementations;

/// <summary>
///     Repository for handling Container entities in the database.
/// </summary>
public class ContainerRepository : IContainerRepository
{
    private readonly ApplicationDbContext _db;

    /// <summary>
    ///     Initializes a new instance of the ContainerRepository class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public ContainerRepository(ApplicationDbContext db)
    {
        _db = db;
    }


    public async Task Create(Container container, CancellationToken ct = default)
    {
        await _db.Containers.AddAsync(container, ct);
    }


    public Task<Container> Get(int containerId, bool trackChanges = false, CancellationToken ct = default)
    {
        return _db.Containers
            .TrackChanges(trackChanges)
            .FirstOrDefaultAsync(x => x.Id == containerId, ct);
    }


    public Task<PaginationResult<Container>> Get(PaginationParams paginationParams, CancellationToken ct = default)
    {
        return _db.Paginate<Container>(paginationParams, ct);
    }


    public void Remove(Container container)
    {
        _db.Containers.Remove(container);
    }

    public async Task<bool> IsNameUnique(string containerName, CancellationToken ct = default)
    {
        return !await _db.Containers.AnyAsync(x => x.Name == containerName, ct);
    }
}