using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Features.Trucks.DTOs;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;
using Fleet.Api.Shared.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Controller for managing Truck entities and related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TrucksController : ControllerBase
{
    private readonly ITruckContainerService _truckContainerService;
    private readonly ITruckService _truckService;

    /// <summary>
    ///     Initializes a new instance of the TrucksController class.
    /// </summary>
    /// <param name="truckService">Service for managing trucks.</param>
    /// <param name="truckContainerService">Service for managing truck containers.</param>
    public TrucksController(ITruckService truckService, ITruckContainerService truckContainerService)
    {
        _truckService = truckService;
        _truckContainerService = truckContainerService;
    }

    /// <summary>
    ///     Creates a new truck with the provided details.
    /// </summary>
    /// <param name="request">The truck creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="201">Returns the Id of the newly created Truck</response>
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
    public async Task<IActionResult> Create(CreateTruckRequest request, CancellationToken ct)
    {
        var result = await _truckService.Create(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Gets details of a specific truck by its Id.
    /// </summary>
    /// <param name="truckId">The Id of the truck.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Action result containing the truck details.</returns>
    /// <response code="200">Returns the truck instance of the specified Id</response>
    /// <response code="404">If the truck does not exist</response>
    [HttpGet("{truckId:int}")]
    [ProducesResponseType(typeof(GetTruckResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int truckId, CancellationToken ct)
    {
        var result = await _truckService.Get(truckId, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Retrieves a paginated list of trucks.
    /// </summary>
    /// <param name="paginationParams">Pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Returns the paginated list of trucks</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<GetTruckResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] PaginationParams paginationParams, CancellationToken ct)
    {
        var result = await _truckService.Get(paginationParams, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Deletes a truck by its Id.
    /// </summary>
    /// <param name="truckId">The Id of the truck to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Action result indicating the success or failure of the operation.</returns>
    /// <response code="200">Returns a success response upon successful deletion.</response>
    /// <response code="404">If the truck does not exist</response>
    [HttpDelete("{truckId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int truckId, CancellationToken ct)
    {
        var result = await _truckService.Delete(truckId, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Loads a truck with the provided container.
    /// </summary>
    /// <param name="truckId">The Id of the truck to load the container onto.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Action result indicating the success or failure of the operation.</returns>
    /// <response code="201">Returns the Id of the newly loaded container on the ship</response>
    /// <response code="400">
    ///     When:
    ///     <ul>
    ///         <li>the [Container] is already loaded in another ship</li>
    ///         <li>the [Truck] is already full</li>
    ///     </ul>
    /// </response>
    /// <response code="404">
    ///     When:
    ///     <ul>
    ///         <li>the [ContainerId] is not found</li>
    ///         <li>the [TruckId] is not found</li>
    ///     </ul>
    /// </response>
    [HttpPost("{truckId}/load")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Load(int truckId, LoadTruckRequest request, CancellationToken ct)
    {
        var result = await _truckContainerService.Load(truckId, request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Unloads a truck with the provided container.
    /// </summary>
    /// <param name="truckId">The Id of the truck to unload the container from.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Action result indicating the success or failure of the operation.</returns>
    /// <response code="200">Returns a success response upon successful unloading.</response>
    /// <response code="400">
    ///     When:
    ///     <ul>
    ///         <li>the [ContainerId] is not loaded in the ship</li>
    ///         <li>the [TruckId] is empty</li>
    ///         <li>the [TruckId] is not unloading its latest container</li>
    ///     </ul>
    /// </response>
    /// <response code="404">
    ///     When:
    ///     <ul>
    ///         <li>the [ContainerId] is not found</li>
    ///     </ul>
    /// </response>
    [HttpPost("{truckId}/unload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unload(int truckId, UnloadTruckRequest request, CancellationToken ct)
    {
        var result = await _truckContainerService.Unload(truckId, request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    ///     Transfers a container from one truck to another.
    /// </summary>
    /// <param name="sourceTruckId">The Id of the source truck.</param>
    /// <param name="destinationTruckId">The Id of the destination truck.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Action result indicating the success or failure of the operation.</returns>
    /// <response code="201">Returns the Id of the newly transferred container on the destination ship</response>
    /// <response code="400">
    ///     When:
    ///     <ul>
    ///         <li>the [ContainerId] not loaded in the source ship</li>
    ///         <li>the [Container] is already in the destination ship</li>
    ///         <li>the [DestinationTruck] is already full</li>
    ///         <li>the [SourceTruckId] is not transferring/unloading its latest container</li>
    ///     </ul>
    /// </response>
    /// <response code="404">
    ///     When:
    ///     <ul>
    ///         <li>the [SourceTruckId] or [DestinationTruckId] is not found</li>
    ///         <li>the [ContainerId] is not found</li>
    ///     </ul>
    /// </response>
    [HttpPost("{sourceTruckId:int}/transfer/{destinationTruckId:int}")]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Transfer(int sourceTruckId, int destinationTruckId,
        TransferContainerRequest request, CancellationToken ct)
    {
        var result = await _truckContainerService.Transfer(sourceTruckId, destinationTruckId, request, ct);
        return result.ToActionResult(this);
    }
}