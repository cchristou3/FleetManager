using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Features.Trucks.DTOs;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Trucks.Abstractions;

public interface ITruckService
{
    /// <summary>
    ///     Creates a new truck based on the provided request.
    /// </summary>
    /// <param name="request">The request containing truck data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the Id of the created truck.</returns>
    Task<Result<int>> Create(CreateTruckRequest request, CancellationToken ct);

    /// <summary>
    ///     Retrieves truck details by Id.
    /// </summary>
    /// <param name="truckId">The Id of the truck to retrieve.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the response containing truck details.</returns>
    Task<Result<GetTruckResponse>> Get(int truckId, CancellationToken ct);

    /// <summary>
    ///     Retrieves a paginated list of trucks.
    /// </summary>
    /// <param name="paginationParams">Parameters for pagination.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the paginated response of truck details.</returns>
    Task<Result<PaginationResponse<GetTruckResponse>>> Get(PaginationParams paginationParams, CancellationToken ct);

    /// <summary>
    ///     Deletes a truck by Id.
    /// </summary>
    /// <param name="truckId">The Id of the truck to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success of the deletion operation.</returns>
    Task<Result<object>> Delete(int truckId, CancellationToken ct);
}