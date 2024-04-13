using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;

namespace Fleet.Api.Features.Ships.Abstractions;

public interface IShipContainerRepository
{
    /// <summary>
    ///     Adds a new Ship Container to the database asynchronously.
    /// </summary>
    /// <param name="shipContainer">The Ship Container to be added.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Create(ShipContainer shipContainer, CancellationToken ct);

    /// <summary>
    ///     Retrieves the Ship Containers of the provided shipId from the database asynchronously.
    /// </summary>
    /// <param name="shipId">The Ship to have its containers fetched.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    ///     A task representing the asynchronous operation containing a list of the
    ///     containers that are loaded on the provided shipId.
    /// </returns>
    Task<int> CountContainersByShipId(int shipId, CancellationToken ct);

    /// <summary>
    ///     Retrieves the Ship Container based on the provided containerId
    /// </summary>
    /// <param name="containerId">The Id of the Container to be searched.</param>
    /// <param name="trackChanges">Indicates whether to track changes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation including the Ship Container (if found).</returns>
    Task<ShipContainer> Get(int containerId, bool trackChanges, CancellationToken ct);

    /// <summary>
    ///     Removes a Ship Container from the database.
    /// </summary>
    /// <param name="shipContainer">The Ship Container to be removed.</param>
    void Remove(ShipContainer shipContainer);
}