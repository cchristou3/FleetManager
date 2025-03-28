using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Features.Real_time;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Ships.DTOs;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Infrastructure;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Requests;
using Microsoft.AspNetCore.SignalR;

namespace Fleet.Api.Features.Ships.Implementations;

/// <summary>
///     Service for managing Ship entities and handling related operations.
/// </summary>
public class ShipContainerService : IShipContainerService
{
    private readonly IContainerRepository _containerRepository;
    private readonly IShipContainerRepository _shipContainerRepository;
    private readonly IShipRepository _shipRepository;
    private readonly ITruckContainerRepository _truckContainerRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IHubContext<ShipHub> _hubContext;

    /// <summary>
    ///     Initializes a new instance of the ShipService class.
    /// </summary>
    /// <param name="shipRepository">Repository for ship data access.</param>
    /// <param name="unitOfWork">Unit of Work for managing transactions.</param>
    /// <param name="containerRepository">Repository for container data access.</param>
    /// <param name="shipContainerRepository">Repository for ship container data access.</param>
    /// <param name="truckContainerRepository">Repository for truck container data access.</param>
    /// <param name="hubContext">Context for real time communication with clients.</param>
    public ShipContainerService(
        IShipRepository shipRepository, IUnitOfWork unitOfWork,
        IContainerRepository containerRepository, IShipContainerRepository shipContainerRepository,
        ITruckContainerRepository truckContainerRepository, IHubContext<ShipHub> hubContext)
    {
        _shipRepository = shipRepository;
        _unitOfWork = unitOfWork;
        _containerRepository = containerRepository;
        _shipContainerRepository = shipContainerRepository;
        _truckContainerRepository = truckContainerRepository;
        _hubContext = hubContext;
    }

    public async Task<Result<int>> Load(int shipId, LoadShipRequest request, string connectionId, CancellationToken ct)
    {
        // TODO: Potential race conditions: add locks to ensure thread safety
        var validationResult = await ValidateLoadRequest(shipId, request, ct);

        if (validationResult.HasFailed) return validationResult.ToResult<int>();

        var shipContainer = ShipContainer.Create(shipId, request);

        await _shipContainerRepository.Create(shipContainer, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        await NotifyContainerLoadedToShip(shipId, validationResult.Value, connectionId, ct);

        return Result<int>.Success(shipContainer.Id);
    }

    public async Task<Result<int>> Unload(int shipId, UnloadShipRequest request, string connectionId, CancellationToken ct)
    {
        var validationResult = await ValidateUnloadRequest(shipId, request, ct);

        if (validationResult.HasFailed) return validationResult;

        var shipContainer = await _shipContainerRepository.Get(request.ContainerId, true, ct);

        _shipContainerRepository.Remove(shipContainer);

        await _unitOfWork.SaveChangesAsync(ct);
        
        await NotifyContainerUnloadedFromShip(shipId, request.ContainerId, connectionId, ct);

        return Result<int>.Success();
    }

    public async Task<Result<int>> Transfer(int sourceShipId, int destinationShipId, TransferContainerRequest request, string connectionId,
        CancellationToken ct)
    {
        // TODO: Potential race conditions: add locks to ensure thread safety
        var validationResult = await ValidateTransferRequest(sourceShipId, destinationShipId, request, ct);

        if (validationResult.HasFailed) return validationResult.ToResult<int>();

        var shipContainer = await _shipContainerRepository.Get(request.ContainerId, true, ct);

        shipContainer.ShipId = destinationShipId;

        await _unitOfWork.SaveChangesAsync(ct);

        await NotifyContainerTransferredToAnotherShip(sourceShipId, destinationShipId, request.ContainerId, connectionId, ct);

        return Result<int>.Success(shipContainer.Id);
    }

    private async Task<Result<int>> ValidateTransferRequest(int sourceShipId, int destinationShipId,
        TransferContainerRequest request, CancellationToken ct)
    {
        if (sourceShipId == destinationShipId)
            return Result<int>.Failure(DomainErrors.ShipContainer.DestinationShipCannotBeSameAsSourceShip);

        var sourceShip = await _shipRepository.Get(sourceShipId, false, ct);

        if (sourceShip is null)
            return Result<int>.Failure(DomainErrors.Ship.SourceShipNotFound);

        var destinationShip = await _shipRepository.Get(destinationShipId, false, ct);

        if (destinationShip is null)
            return Result<int>.Failure(DomainErrors.Ship.DestinationShipNotFound);

        var shipContainer = await _shipContainerRepository.Get(request.ContainerId, true, ct);

        if (shipContainer is null)
            return Result<int>.Failure(DomainErrors.Container.NotFound);

        if (shipContainer.ShipId != sourceShip.Id)
        {
            if (shipContainer.ShipId == destinationShip.Id)
                return Result<int>.Failure(DomainErrors.ShipContainer.AlreadyInDestination);

            return Result<int>.Failure(DomainErrors.ShipContainer.NotInSource);
        }

        var numberOfShipContainers = await _shipContainerRepository.CountContainersByShipId(destinationShipId, ct);
        if (destinationShip.IsFull(numberOfShipContainers))
            return Result<int>.Failure(DomainErrors.Ship.DestinationShipIsFull);

        return Result<int>.Success();
    }

    /// <summary>
    ///     Validates the request for loading a ship with a container.
    /// </summary>
    /// <param name="shipId">The Id of the ship that will load the provided container.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the validation.</returns>
    private async Task<Result<Container>> ValidateLoadRequest(int shipId, LoadShipRequest request,
        CancellationToken ct = default)
    {
        var ship = await _shipRepository.Get(shipId, false, ct);

        if (ship is null)
            return Result<Container>.Failure(DomainErrors.Ship.NotFound);

        var numberOfShipContainers = await _shipContainerRepository.CountContainersByShipId(shipId, ct);

        if (ship.IsFull(numberOfShipContainers))
            return Result<Container>.Failure(DomainErrors.Ship.IsFull);

        var container = await _containerRepository.Get(request.ContainerId, false, ct);

        if (container is null)
            return Result<Container>.Failure(DomainErrors.Container.NotFound);

        var truckContainer = await _truckContainerRepository.Get(request.ContainerId, false, ct);
        if (truckContainer != null)
            return Result<Container>.Failure(DomainErrors.Container.LoadedInTruck);

        var shipContainer = await _shipContainerRepository.Get(request.ContainerId, false, ct);
        if (shipContainer != null) return Result<Container>.Failure(DomainErrors.Container.LoadedInShip);

        return Result<Container>.Success(container);
    }

    /// <summary>
    ///     Validates the request for unloading a ship with a container.
    /// </summary>
    /// <param name="shipId">The Id of the ship that will unload the provided container.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the validation.</returns>
    private async Task<Result<int>> ValidateUnloadRequest(int shipId, UnloadShipRequest request,
        CancellationToken ct = default)
    {
        var shipContainer = await _shipContainerRepository.Get(request.ContainerId, true, ct);

        if (shipContainer is null)
            return Result<int>.Failure(DomainErrors.Container.NotLoaded);

        if (shipContainer.ShipId != shipId)
            return Result<int>.Failure(DomainErrors.ShipContainer.LoadedInAnotherShip);

        return Result<int>.Success();
    }
    
    private Task NotifyContainerLoadedToShip(int shipId, Container container, string connectionId, CancellationToken ct)
    {
        return _hubContext.Clients
            .AllExcept(connectionId)
            .SendAsync(nameof(ClientMethods.ShipLoaded), new OnShipLoadedEvent(shipId, container), ct);
    }
    
    private async Task NotifyContainerUnloadedFromShip(int shipId, int containerId, string connectionId, CancellationToken ct)
    {
        var container = await _containerRepository.Get(containerId, false, ct);
        
        await _hubContext.Clients
            .AllExcept(connectionId)
            .SendAsync(nameof(ClientMethods.ShipUnloaded), new OnShipUnloadedEvent(shipId, container), ct);
    }
    
    private async Task NotifyContainerTransferredToAnotherShip(int sourceShipId, int destinationShipId, int containerId, string connectionId, CancellationToken ct)
    {
        var container = await _containerRepository.Get(containerId, false, ct);
        
        await _hubContext.Clients
            .AllExcept(connectionId)
            .SendAsync(nameof(ClientMethods.ContainerTransferred), new OnContainerTransferredEvent(sourceShipId, destinationShipId, container), ct);
    }
}