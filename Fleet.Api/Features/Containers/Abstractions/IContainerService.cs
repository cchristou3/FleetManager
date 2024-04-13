using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Features.Containers.DTOs;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Containers.Abstractions;

public interface IContainerService
{
    /// <summary>
    ///     Creates a new container based on the provided request.
    /// </summary>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the Id of the created container.</returns>
    Task<Result<int>> Create(CreateContainerRequest request, CancellationToken ct);

    /// <summary>
    ///     Retrieves container details by Id.
    /// </summary>
    /// <param name="containerId">The Id of the container to retrieve.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the response containing container details.</returns>
    Task<Result<GetContainerResponse>> Get(int containerId, CancellationToken ct);

    /// <summary>
    ///     Retrieves a paginated list of containers.
    /// </summary>
    /// <param name="paginationParams">Parameters for pagination.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success and the paginated response of container details.</returns>
    Task<Result<PaginationResponse<GetContainerResponse>>> Get(PaginationParams paginationParams, CancellationToken ct);

    /// <summary>
    ///     Deletes a container by Id.
    /// </summary>
    /// <param name="containerId">The Id of the container to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success of the deletion operation.</returns>
    Task<Result<object>> Delete(int containerId, CancellationToken ct);
}