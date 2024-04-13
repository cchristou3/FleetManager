using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Ships.Abstractions;

public interface IShipRepository
{
    /// <summary>
    ///     Adds a new Ship to the database asynchronously.
    /// </summary>
    /// <param name="ship">The Ship to be added.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Create(Ship ship, CancellationToken ct = default);


    /// <summary>
    ///     Retrieves a Ship by its Id with optional change tracking.
    /// </summary>
    /// <param name="shipId">The Id of the Ship to retrieve.</param>
    /// <param name="trackChanges">Indicates whether to track changes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and the retrieved Ship.</returns>
    Task<Ship> Get(int shipId, bool trackChanges = false, CancellationToken ct = default);

    /// <summary>
    ///     Retrieves Ships with pagination support.
    /// </summary>
    /// <param name="paginationParams">Parameters for pagination.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and the paginated result of Ships.</returns>
    Task<PaginationResult<Ship>> Get(PaginationParams paginationParams, CancellationToken ct = default);

    /// <summary>
    ///     Removes a Ship from the database.
    /// </summary>
    /// <param name="ship">The Ship to be removed.</param>
    void Remove(Ship ship);

    /// <summary>
    ///     Checks if a Ship name is unique in the database.
    /// </summary>
    /// <param name="shipName">The name to check for uniqueness.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and a boolean indicating the uniqueness of the name.</returns>
    Task<bool> IsNameUnique(string shipName, CancellationToken ct = default);
}