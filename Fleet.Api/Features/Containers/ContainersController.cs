using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Features.Containers.DTOs;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fleet.Api.Features.Containers;

[ApiController]
[Route("api/[controller]")]
public class ContainersController : ControllerBase
{
    private readonly IContainerService _containerService;

    public ContainersController(IContainerService containerService)
    {
        _containerService = containerService;
    }

    /// <summary>
    ///     Creates a new container with the provided name.
    /// </summary>
    /// <response code="201">Returns the Id of the newly created Container</response>
    /// <response code="400">
    ///     When:
    ///     <ul>
    ///         <li>the [Name] is null or empty</li>
    ///         <li>the [Name] is exceeds 100 characters</li>
    ///         <li>the [Name] is already taken</li>
    ///     </ul>
    /// </response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateContainerRequest request, CancellationToken ct)
    {
        var result = await _containerService.Create(request, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Retrieves the container with the specified Id.
    /// </summary>
    /// <response code="200">Returns the container instance of the specified Id</response>
    /// <response code="404">
    ///     If the container does not exists
    /// </response>
    [HttpGet("{containerId:int}")]
    [ProducesResponseType(typeof(GetContainerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int containerId, CancellationToken ct)
    {
        var result = await _containerService.Get(containerId, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Retrieves a paginated list of containers.
    /// </summary>
    /// <param name="paginationParams">Pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns the paginated list of containers</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<GetContainerResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] PaginationParams paginationParams, CancellationToken ct)
    {
        var result = await _containerService.Get(paginationParams, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Deletes a container by its identifier.
    /// </summary>
    /// <param name="containerId">The unique identifier of the container to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns a success response upon successful deletion.</response>
    /// <response code="404">If the container does not exist</response>
    /// <response code="404">If the container is loaded in either a truck or a ship</response>
    [HttpDelete("{containerId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int containerId, CancellationToken ct)
    {
        var result = await _containerService.Delete(containerId, ct);

        return result.ToActionResult(this);
    }
}