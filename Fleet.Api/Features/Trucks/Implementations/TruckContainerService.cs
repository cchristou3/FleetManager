using System;
using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Features.Trucks.DTOs;
using Fleet.Api.Infrastructure;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Requests;

namespace Fleet.Api.Features.Trucks.Implementations;

/// <summary>
///     Service for managing Truck entities and handling related operations.
/// </summary>
public class TruckContainerService : ITruckContainerService
{
    private readonly IContainerRepository _containerRepository;
    private readonly IShipContainerRepository _shipContainerRepository;
    private readonly ITruckContainerRepository _truckContainerRepository;
    private readonly ITruckRepository _truckRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    ///     Initializes a new instance of the TruckService class.
    /// </summary>
    /// <param name="truckRepository">Repository for truck data access.</param>
    /// <param name="unitOfWork">Unit of Work for managing transactions.</param>
    /// <param name="containerRepository">Repository for container data access.</param>
    /// <param name="truckContainerRepository">Repository for truck container data access.</param>
    public TruckContainerService(
        ITruckRepository truckRepository, IUnitOfWork unitOfWork,
        IContainerRepository containerRepository, ITruckContainerRepository truckContainerRepository,
        IShipContainerRepository shipContainerRepository)
    {
        _truckRepository = truckRepository;
        _unitOfWork = unitOfWork;
        _containerRepository = containerRepository;
        _truckContainerRepository = truckContainerRepository;
        _shipContainerRepository = shipContainerRepository;
    }

    /// <summary>
    ///     Loads an existing truck with the provided container.
    /// </summary>
    /// <param name="truckId">The Id of the truck to have the provided container loaded.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success regarding the load operation.</returns>
    public async Task<Result<int>> Load(int truckId, LoadTruckRequest request, CancellationToken ct)
    {
        // TODO: Potential race conditions: add locks to ensure thread safety
        var validationResult = await ValidateLoadRequest(truckId, request, ct);

        if (validationResult.HasFailed) return validationResult;

        var truckContainer = TruckContainer.Create(truckId, request);

        await _truckContainerRepository.Create(truckContainer, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(truckContainer.Id);
    }

    public async Task<Result<int>> Unload(int truckId, UnloadTruckRequest request, CancellationToken ct)
    {
        var validationResult = await ValidateUnloadRequest(truckId, request, ct);

        if (validationResult.HasFailed) return validationResult;

        var truckContainer = await _truckContainerRepository.Get(request.ContainerId, true, ct);

        _truckContainerRepository.Remove(truckContainer);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success();
    }

    public async Task<Result<int>> Transfer(int sourceTruckId, int destinationTruckId, TransferContainerRequest request,
        CancellationToken ct)
    {
        // TODO: Potential race conditions: add locks to ensure thread safety
        var validationResult = await ValidateTransferRequest(sourceTruckId, destinationTruckId, request, ct);

        if (validationResult.HasFailed) return validationResult;

        var truckContainer = await _truckContainerRepository.Get(request.ContainerId, true, ct);

        truckContainer.TruckId = destinationTruckId;
        truckContainer.DateLoaded = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(truckContainer.Id);
    }

    private async Task<Result<int>> ValidateTransferRequest(int sourceTruckId, int destinationTruckId,
        TransferContainerRequest request, CancellationToken ct)
    {
        if (sourceTruckId == destinationTruckId)
            return Result<int>.Failure(DomainErrors.TruckContainer.DestinationTruckCannotBeSameAsSourceTruck);

        var sourceTruck = await _truckRepository.Get(sourceTruckId, false, ct);

        if (sourceTruck is null)
            return Result<int>.Failure(DomainErrors.Truck.SourceTruckNotFound);

        var destinationTruck = await _truckRepository.Get(destinationTruckId, false, ct);

        if (destinationTruck is null)
            return Result<int>.Failure(DomainErrors.Truck.DestinationTruckNotFound);

        var truckContainer = await _truckContainerRepository.Get(request.ContainerId, true, ct);

        if (truckContainer is null)
            return Result<int>.Failure(DomainErrors.Container.NotFound);

        if (truckContainer.TruckId != sourceTruck.Id)
        {
            if (truckContainer.TruckId == destinationTruck.Id)
                return Result<int>.Failure(DomainErrors.TruckContainer.AlreadyInDestination);

            return Result<int>.Failure(DomainErrors.TruckContainer.NotInSource);
        }

        var numberOfTruckContainers = await _truckContainerRepository.CountContainersByTruckId(destinationTruckId, ct);
        if (destinationTruck.IsFull(numberOfTruckContainers))
            return Result<int>.Failure(DomainErrors.Truck.DestinationTruckIsFull);

        var latestLoadedContainer = await _truckContainerRepository.GetLatestLoaded(sourceTruckId, false, ct);

        if (latestLoadedContainer is null)
            return Result<int>.Failure(DomainErrors.Truck.SourceIsEmpty);

        if (latestLoadedContainer.ContainerId != request.ContainerId)
            return Result<int>.Failure(DomainErrors.TruckContainer.NotLatestLoaded);

        return Result<int>.Success();
    }

    /// <summary>
    ///     Validates the request for loading a truck with a container.
    /// </summary>
    /// <param name="truckId">The Id of the truck that will load the provided container.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the validation.</returns>
    private async Task<Result<int>> ValidateLoadRequest(int truckId, LoadTruckRequest request,
        CancellationToken ct = default)
    {
        var truck = await _truckRepository.Get(truckId, false, ct);

        if (truck is null)
            return Result<int>.Failure(DomainErrors.Truck.NotFound);

        var numberOfTruckContainers = await _truckContainerRepository.CountContainersByTruckId(truckId, ct);

        if (truck.IsFull(numberOfTruckContainers))
            return Result<int>.Failure(DomainErrors.Truck.IsFull);

        var container = await _containerRepository.Get(request.ContainerId, false, ct);

        if (container is null)
            return Result<int>.Failure(DomainErrors.Container.NotFound);

        var truckContainer = await _truckContainerRepository.Get(request.ContainerId, false, ct);
        if (truckContainer != null)
        {
            if (truckContainer.TruckId == truckId)
                return Result<int>.Failure(DomainErrors.TruckContainer.AlreadyInDestination);

            return Result<int>.Failure(DomainErrors.TruckContainer.LoadedInAnotherTruck);
        }

        var shipContainer = await _shipContainerRepository.Get(request.ContainerId, false, ct);
        if (shipContainer != null)
            return Result<int>.Failure(DomainErrors.Container.LoadedInShip);

        return Result<int>.Success();
    }

    /// <summary>
    ///     Validates the request for unloading a truck with a container.
    /// </summary>
    /// <param name="truckId">The Id of the truck that will unload the provided container.</param>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the validation.</returns>
    private async Task<Result<int>> ValidateUnloadRequest(int truckId, UnloadTruckRequest request,
        CancellationToken ct = default)
    {
        var truckContainer = await _truckContainerRepository.Get(request.ContainerId, true, ct);

        if (truckContainer is null)
            return Result<int>.Failure(DomainErrors.Container.NotLoaded);

        if (truckContainer.TruckId != truckId)
            return Result<int>.Failure(DomainErrors.TruckContainer.LoadedInAnotherTruck);

        var latestLoadedContainer = await _truckContainerRepository.GetLatestLoaded(truckId, false, ct);

        if (latestLoadedContainer is null)
            return Result<int>.Failure(DomainErrors.Truck.IsEmpty);

        if (latestLoadedContainer.ContainerId != request.ContainerId)
            return Result<int>.Failure(DomainErrors.TruckContainer.NotLatestLoaded);

        return Result<int>.Success();
    }
}