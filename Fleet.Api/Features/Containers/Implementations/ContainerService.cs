using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Features.Containers.DTOs;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Infrastructure;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Containers.Implementations;

/// <summary>
///     Service for managing Container entities and handling related operations.
/// </summary>
public class ContainerService : IContainerService
{
    /// <summary>
    ///     Maximum length allowed for a container name.
    /// </summary>
    public const int ContainerNameMaximumLength = 100;

    private readonly IContainerRepository _containerRepository;
    private readonly IMapper _mapper;
    private readonly IShipContainerRepository _shipContainerRepository;
    private readonly ITruckContainerRepository _truckContainerRepository;

    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    ///     Initializes a new instance of the ContainerService class.
    /// </summary>
    /// <param name="containerRepository">Repository for container data access.</param>
    /// <param name="shipContainerRepository">Repository for ship container data access.</param>
    /// <param name="truckContainerRepository">Repository for truck container data access.</param>
    /// <param name="unitOfWork">Unit of Work for managing transactions.</param>
    /// <param name="mapper">Mapper for DTO transformations.</param>
    public ContainerService(
        IContainerRepository containerRepository, IShipContainerRepository shipContainerRepository,
        ITruckContainerRepository truckContainerRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _containerRepository = containerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _shipContainerRepository = shipContainerRepository;
        _truckContainerRepository = truckContainerRepository;
    }

    /// <summary>
    ///     Creates a new container based on the provided request.
    /// </summary>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the create operation.</returns>
    public async Task<Result<int>> Create(CreateContainerRequest request, CancellationToken ct = default)
    {
        var validationResult = await ValidateCreateRequest(request, ct);

        if (validationResult.HasFailed) return validationResult;

        var container = Container.Create(request);

        await _containerRepository.Create(container, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(container.Id);
    }

    /// <summary>
    ///     Gets details of a specific container by its Id.
    /// </summary>
    /// <param name="containerId">The Id of the container.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the container details or an error if not found.</returns>
    public async Task<Result<GetContainerResponse>> Get(int containerId, CancellationToken ct)
    {
        var container = await _containerRepository.Get(containerId, false, ct);

        if (container is null) return Result<GetContainerResponse>.Failure(DomainErrors.Container.NotFound);

        var response = GetContainerResponse.Create(container);
        return Result<GetContainerResponse>.Success(response);
    }

    /// <summary>
    ///     Gets a paginated list of containers based on the provided parameters.
    /// </summary>
    /// <param name="paginationParams">Pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the paginated list of containers or an error if encountered.</returns>
    public async Task<Result<PaginationResponse<GetContainerResponse>>> Get(PaginationParams paginationParams,
        CancellationToken ct)
    {
        var paginationResult = await _containerRepository.Get(paginationParams, ct);

        var transformedPagedItems = _mapper.Map<List<Container>, List<GetContainerResponse>>(paginationResult);

        var response =
            PaginationResponse<GetContainerResponse>
                .Create(transformedPagedItems, paginationResult.TotalCount, paginationParams);

        return Result<PaginationResponse<GetContainerResponse>>.Success(response);
    }

    /// <summary>
    ///     Deletes a container by its Id.
    /// </summary>
    /// <param name="containerId">The Id of the container to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the delete operation.</returns>
    public async Task<Result<object>> Delete(int containerId, CancellationToken ct)
    {
        var container = await _containerRepository.Get(containerId, true, ct);

        if (container is null)
            return Result<object>.Failure(DomainErrors.Container.NotFound);

        var shipContainer = await _shipContainerRepository.Get(containerId, false, ct);

        if (shipContainer is not null)
            return Result<object>.Failure(DomainErrors.Container.LoadedInShip);

        var truckContainer = await _truckContainerRepository.Get(containerId, false, ct);

        if (truckContainer is not null)
            return Result<object>.Failure(DomainErrors.Container.LoadedInTruck);

        _containerRepository.Remove(container);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<object>.Success();
    }

    /// <summary>
    ///     Validates the create container request.
    /// </summary>
    /// <param name="request">The request containing container data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the validation.</returns>
    private async Task<Result<int>> ValidateCreateRequest(CreateContainerRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(request.Name))
            return Result<int>.Failure(DomainErrors.Container.NameCannotBeEmpty);

        if (request.Name.Length > ContainerNameMaximumLength)
            return Result<int>.Failure(DomainErrors.Container.TooLong(ContainerNameMaximumLength));

        if (!await _containerRepository.IsNameUnique(request.Name, ct))
            return Result<int>.Failure(DomainErrors.Container.NameMustBeUnique);

        return Result<int>.Success();
    }
}