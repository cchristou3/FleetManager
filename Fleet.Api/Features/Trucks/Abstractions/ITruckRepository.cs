using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Trucks.Abstractions;

public interface ITruckRepository
{
    /// <summary>
    ///     Adds a new Truck to the database asynchronously.
    /// </summary>
    /// <param name="truck">The Truck to be added.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Create(Truck truck, CancellationToken ct = default);


    /// <summary>
    ///     Retrieves a Truck by its Id with optional change tracking.
    /// </summary>
    /// <param name="truckId">The Id of the Truck to retrieve.</param>
    /// <param name="trackChanges">Indicates whether to track changes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and the retrieved Truck.</returns>
    Task<Truck> Get(int truckId, bool trackChanges = false, CancellationToken ct = default);

    /// <summary>
    ///     Retrieves Trucks with pagination support.
    /// </summary>
    /// <param name="paginationParams">Parameters for pagination.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and the paginated result of Trucks.</returns>
    Task<PaginationResult<Truck>> Get(PaginationParams paginationParams, CancellationToken ct = default);

    /// <summary>
    ///     Removes a Truck from the database.
    /// </summary>
    /// <param name="truck">The Truck to be removed.</param>
    void Remove(Truck truck);

    /// <summary>
    ///     Checks if a Truck name is unique in the database.
    /// </summary>
    /// <param name="truckName">The name to check for uniqueness.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and a boolean indicating the uniqueness of the name.</returns>
    Task<bool> IsNameUnique(string truckName, CancellationToken ct = default);
}