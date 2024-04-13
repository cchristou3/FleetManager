using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Features.Trucks.DTOs;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Requests;

namespace Fleet.Api.Features.Trucks.Abstractions;

public interface ITruckContainerService
{
    /// <summary>
    ///     Loads an existing truck with the provided container.
    /// </summary>
    /// <param name="truckId">The truck Id to have the provided container loaded.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success regarding the load operation.</returns>
    Task<Result<int>> Load(int truckId, LoadTruckRequest request, CancellationToken ct);

    /// <summary>
    ///     Unloads the provided container from an existing truck.
    /// </summary>
    /// <param name="truckId">The truck Id to have the provided container unloaded.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success regarding the unload operation.</returns>
    Task<Result<int>> Unload(int truckId, UnloadTruckRequest request, CancellationToken ct);

    /// <summary>
    ///     Transfers a container from one truck to another.
    /// </summary>
    /// <param name="sourceTruckId">The Id of the truck to have the container transferred from.</param>
    /// <param name="destinationTruckId">The Id of the truck to have the container transferred to.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success regarding the load operation.</returns>
    Task<Result<int>> Transfer(int sourceTruckId, int destinationTruckId, TransferContainerRequest request,
        CancellationToken ct);
}