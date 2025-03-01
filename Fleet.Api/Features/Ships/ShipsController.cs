using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Ships.DTOs;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;
using Fleet.Api.Shared.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fleet.Api.Features.Ships;

[ApiController]
[Route("api/[controller]")]
public class ShipsController : ControllerBase
{
    private readonly IShipContainerService _shipContainerService;
    private readonly IShipService _shipService;

    public ShipsController(IShipService shipService, IShipContainerService shipContainerService)
    {
        _shipService = shipService;
        _shipContainerService = shipContainerService;
    }

    /// <summary>
    ///     Creates a new ship with the provided details.
    /// </summary>
    /// <param name="request">The ship creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="201">Returns the Id of the newly created Ship</response>
    /// <response code="400">
    ///     When:
    ///     <ul>
    ///         <li>the [Name] is null or empty</li>
    ///         <li>the [Name] exceeds 100 characters</li>
    ///         <li>the [Capacity] is out of bounds</li>
    ///         <li>the [Name] is already taken</li>
    ///     </ul>
    /// </response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateShipRequest request,
        CancellationToken ct)
    {
        var result = await _shipService.Create(request, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Retrieves details of a specific ship by its identifier.
    /// </summary>
    /// <param name="shipId">The unique identifier of the ship.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns the ship instance of the specified Id</response>
    /// <response code="404">If the ship does not exist</response>
    [HttpGet("{shipId:int}")]
    [ProducesResponseType(typeof(GetShipResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        int shipId,
        CancellationToken ct)
    {
        var result = await _shipService.Get(shipId, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Retrieves a paginated list of ships.
    /// </summary>
    /// <param name="paginationParams">Pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns the paginated list of ships</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<GetShipResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken ct)
    {
        var result = await _shipService.Get(paginationParams, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Deletes a ship by its identifier.
    /// </summary>
    /// <param name="shipId">The unique identifier of the ship to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns a success response upon successful deletion.</response>
    /// <response code="404">If the ship does not exist</response>
    [HttpDelete("{shipId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int shipId,
        CancellationToken ct)
    {
        var result = await _shipService.Delete(shipId, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Loads an existing ship with the provided container.
    /// </summary>
    /// <param name="shipId">The Id of the ship to have the provided container loaded.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="headers">Custom header attributes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="201">Returns the Id of the newly loaded container on the ship</response>
    /// <response code="400">
    ///     When:
    ///     <ul>
    ///         <li>the [Container] is already loaded in another ship</li>
    ///         <li>the [Ship] is already full</li>
    ///     </ul>
    /// </response>
    /// <response code="404">
    ///     When:
    ///     <ul>
    ///         <li>the [ContainerId] is not found</li>
    ///     </ul>
    /// </response>
    [HttpPost("{shipId}/load")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Load(
        int shipId,
        LoadShipRequest request,
        [FromHeader] Headers headers,
        CancellationToken ct)
    {
        var result = await _shipContainerService.Load(shipId, request, headers.ConnectionId, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Unloads a container from a ship.
    /// </summary>
    /// <param name="shipId">The Id of the ship to unload the provided container from.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="headers">Custom header attributes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns a success response upon successful unloading.</response>
    /// <response code="400">
    ///     When:
    ///     <ul>
    ///         <li>the [ContainerId] is not loaded in the ship</li>
    ///     </ul>
    /// </response>
    /// <response code="404">
    ///     When:
    ///     <ul>
    ///         <li>the [ContainerId] is not found</li>
    ///     </ul>
    /// </response>
    [HttpPost("{shipId}/unload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unload(
        int shipId,
        UnloadShipRequest request,
        [FromHeader] Headers headers,
        CancellationToken ct)
    {
        var result = await _shipContainerService.Unload(shipId, request, headers.ConnectionId, ct);

        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Transfers a container from one ship to another.
    /// </summary>
    /// <param name="sourceShipId">The Id of the source ship.</param>
    /// <param name="destinationShipId">The Id of the destination ship.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="headers">Custom header attributes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="201">Returns the Id of the newly transferred container on the destination ship</response>
    /// <response code="400">
    ///     When:
    ///     <ul>
    ///         <li>the [ContainerId] not loaded in the source ship</li>
    ///         <li>the [Container] is already in the destination ship</li>
    ///         <li>the [DestinationShip] is already full</li>
    ///     </ul>
    /// </response>
    /// <response code="404">
    ///     When:
    ///     <ul>
    ///         <li>the [SourceShipId] or [DestinationShipId] is not found</li>
    ///         <li>the [ContainerId] is not found</li>
    ///     </ul>
    /// </response>
    [HttpPost("{sourceShipId:int}/transfer/{destinationShipId:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Transfer(
        int sourceShipId,
        int destinationShipId,
        TransferContainerRequest request,
        [FromHeader] Headers headers,
        CancellationToken ct)
    {
        var result = await _shipContainerService.Transfer(sourceShipId, destinationShipId, request, headers.ConnectionId, ct);

        return result.ToActionResult(this);
    }
}