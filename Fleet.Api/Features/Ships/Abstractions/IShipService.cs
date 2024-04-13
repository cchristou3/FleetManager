using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Features.Ships.DTOs;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Ships.Abstractions;

public interface IShipService
{
    /// <summary>
    ///     Creates a new ship based on the provided request.
    /// </summary>
    /// <param name="request">The request containing ship data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the Id of the created ship.</returns>
    Task<Result<int>> Create(CreateShipRequest request, CancellationToken ct);

    /// <summary>
    ///     Retrieves ship details by Id.
    /// </summary>
    /// <param name="shipId">The Id of the ship to retrieve.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the response containing ship details.</returns>
    Task<Result<GetShipResponse>> Get(int shipId, CancellationToken ct);

    /// <summary>
    ///     Retrieves a paginated list of ships.
    /// </summary>
    /// <param name="paginationParams">Parameters for pagination.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the paginated response of ship details.</returns>
    Task<Result<PaginationResponse<GetShipResponse>>> Get(PaginationParams paginationParams, CancellationToken ct);

    /// <summary>
    ///     Deletes a ship by Id.
    /// </summary>
    /// <param name="shipId">The Id of the ship to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success of the deletion operation.</returns>
    Task<Result<object>> Delete(int shipId, CancellationToken ct);
}