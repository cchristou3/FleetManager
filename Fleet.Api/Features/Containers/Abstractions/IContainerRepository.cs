using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Containers.Abstractions;

public interface IContainerRepository
{
    /// <summary>
    ///     Adds a new Container to the database asynchronously.
    /// </summary>
    /// <param name="container">The Container to be added.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Create(Container container, CancellationToken ct = default);


    /// <summary>
    ///     Retrieves a Container by its Id with optional change tracking.
    /// </summary>
    /// <param name="containerId">The Id of the Container to retrieve.</param>
    /// <param name="trackChanges">Indicates whether to track changes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and the retrieved Container.</returns>
    Task<Container> Get(int containerId, bool trackChanges = false, CancellationToken ct = default);

    /// <summary>
    ///     Retrieves Containers with pagination support.
    /// </summary>
    /// <param name="paginationParams">Parameters for pagination.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and the paginated result of Containers.</returns>
    Task<PaginationResult<Container>> Get(PaginationParams paginationParams, CancellationToken ct = default);

    /// <summary>
    ///     Removes a Container from the database.
    /// </summary>
    /// <param name="container">The Container to be removed.</param>
    void Remove(Container container);

    /// <summary>
    ///     Checks if a Container name is unique in the database.
    /// </summary>
    /// <param name="containerName">The name to check for uniqueness.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation and a boolean indicating the uniqueness of the name.</returns>
    Task<bool> IsNameUnique(string containerName, CancellationToken ct = default);
}