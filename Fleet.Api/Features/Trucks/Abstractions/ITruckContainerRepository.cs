using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;

namespace Fleet.Api.Features.Trucks.Abstractions;

public interface ITruckContainerRepository
{
    /// <summary>
    ///     Adds a new Truck Container to the database asynchronously.
    /// </summary>
    /// <param name="truckContainer">The Truck Container to be added.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Create(TruckContainer truckContainer, CancellationToken ct);

    /// <summary>
    ///     Retrieves the Truck Containers of the provided truckId from the database asynchronously.
    /// </summary>
    /// <param name="truckId">The Truck to have its containers fetched.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    ///     A task representing the asynchronous operation containing a list of the
    ///     containers that are loaded on the provided truckId.
    /// </returns>
    Task<int> CountContainersByTruckId(int truckId, CancellationToken ct);

    /// <summary>
    ///     Retrieves the Truck Container based on the provided containerId
    /// </summary>
    /// <param name="containerId">The Id of the Container to be searched.</param>
    /// <param name="trackChanges">Indicates whether to track changes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation including the Truck Container (if found).</returns>
    Task<TruckContainer> Get(int containerId, bool trackChanges, CancellationToken ct);

    /// <summary>
    ///     Retrieves the Truck's latest loaded Container.
    /// </summary>
    /// <param name="truckId">The Id of the Truck whose latest loaded container we are interested.</param>
    /// <param name="trackChanges">Indicates whether to track changes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation including the Truck Container (if found).</returns>
    Task<TruckContainer> GetLatestLoaded(int truckId, bool trackChanges, CancellationToken ct);

    /// <summary>
    ///     Removes a Truck Container from the database.
    /// </summary>
    /// <param name="truckContainer">The Truck Container to be removed.</param>
    void Remove(TruckContainer truckContainer);
}