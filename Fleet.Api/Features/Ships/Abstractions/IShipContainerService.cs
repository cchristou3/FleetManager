using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Features.Ships.DTOs;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Requests;

namespace Fleet.Api.Features.Ships.Abstractions;

public interface IShipContainerService
{
    /// <summary>
    ///     Loads an existing ship with the provided container.
    /// </summary>
    /// <param name="shipId">The ship Id to have the provided container loaded.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="connectionId">The SignalR connection Id of the caller.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success regarding the load operation.</returns>
    Task<Result<int>> Load(int shipId, LoadShipRequest request, string connectionId, CancellationToken ct);

    /// <summary>
    ///     Unloads the provided container from an existing ship.
    /// </summary>
    /// <param name="shipId">The ship Id to have the provided container unloaded.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success regarding the unload operation.</returns>
    Task<Result<int>> Unload(int shipId, UnloadShipRequest request, CancellationToken ct);

    /// <summary>
    ///     Transfers a container from one ship to another.
    /// </summary>
    /// <param name="sourceShipId">The Id of the ship to have the container transferred from.</param>
    /// <param name="destinationShipId">The Id of the ship to have the container transferred to.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success regarding the load operation.</returns>
    Task<Result<int>> Transfer(int sourceShipId, int destinationShipId, TransferContainerRequest request,
        CancellationToken ct);
}