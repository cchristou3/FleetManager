using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Features.Trucks.DTOs;
using Fleet.Api.Infrastructure;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Trucks.Implementations;

/// <summary>
///     Service responsible for managing Truck entities and handling related operations.
/// </summary>
public class TruckService : ITruckService
{
    /// <summary>
    ///     Maximum length allowed for a truck name.
    /// </summary>
    public const int TruckNameMaximumLength = 100;

    /// <summary>
    ///     Maximum number of containers allowed for a truck to hold.
    /// </summary>
    public const int TruckMaximumCapacity = 3;

    private readonly IMapper _mapper;
    private readonly ITruckRepository _truckRepository;

    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    ///     Initializes a new instance of the TruckService class.
    /// </summary>
    /// <param name="truckRepository">Repository for truck data access.</param>
    /// <param name="unitOfWork">Unit of Work for managing transactions.</param>
    /// <param name="mapper">Mapper for DTO transformations.</param>
    public TruckService(ITruckRepository truckRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _truckRepository = truckRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    ///     Creates a new truck based on the provided request.
    /// </summary>
    /// <param name="request">The request containing truck data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the create operation.</returns>
    public async Task<Result<int>> Create(CreateTruckRequest request, CancellationToken ct = default)
    {
        var validationResult = await ValidateCreateRequest(request, ct);

        if (validationResult.HasFailed) return validationResult;

        var truck = Truck.Create(request);

        await _truckRepository.Create(truck, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(truck.Id);
    }

    /// <summary>
    ///     Gets details of a specific truck by its Id.
    /// </summary>
    /// <param name="truckId">The Id of the truck.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the truck details or an error if not found.</returns>
    public async Task<Result<GetTruckResponse>> Get(int truckId, CancellationToken ct)
    {
        var truck = await _truckRepository.Get(truckId, false, ct);

        if (truck is null) return Result<GetTruckResponse>.Failure(DomainErrors.Truck.NotFound);

        var response = _mapper.Map<Truck, GetTruckResponse>(truck);
        return Result<GetTruckResponse>.Success(response);
    }

    /// <summary>
    ///     Gets a paginated list of trucks based on the provided parameters.
    /// </summary>
    /// <param name="paginationParams">Pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the paginated list of trucks or an error if encountered.</returns>
    public async Task<Result<PaginationResponse<GetTruckResponse>>> Get(PaginationParams paginationParams,
        CancellationToken ct)
    {
        var paginationResult = await _truckRepository.Get(paginationParams, ct);

        var transformedPagedItems = _mapper.Map<List<Truck>, List<GetTruckResponse>>(paginationResult);

        var response =
            PaginationResponse<GetTruckResponse>
                .Create(transformedPagedItems, paginationResult.TotalCount, paginationParams);

        return Result<PaginationResponse<GetTruckResponse>>.Success(response);
    }

    /// <summary>
    ///     Deletes a truck by its Id.
    /// </summary>
    /// <param name="truckId">The Id of the truck to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the delete operation.</returns>
    public async Task<Result<object>> Delete(int truckId, CancellationToken ct)
    {
        var truck = await _truckRepository.Get(truckId, true, ct);

        if (truck is null) return Result<object>.Failure(DomainErrors.Truck.NotFound);

        _truckRepository.Remove(truck);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<object>.Success();
    }

    /// <summary>
    ///     Validates the create truck request.
    /// </summary>
    /// <param name="request">The request containing truck data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the validation.</returns>
    private async Task<Result<int>> ValidateCreateRequest(CreateTruckRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(request.Name))
            return Result<int>.Failure(DomainErrors.Truck.NameCannotBeEmpty);

        if (request.Name.Length > TruckNameMaximumLength)
            return Result<int>.Failure(DomainErrors.Truck.TooLong(TruckNameMaximumLength));

        if (request.IsCapacityOutOfBounds(TruckMaximumCapacity))
            return Result<int>.Failure(DomainErrors.Truck.CapacityOutOfBounds(TruckMaximumCapacity));

        if (!await _truckRepository.IsNameUnique(request.Name, ct))
            return Result<int>.Failure(DomainErrors.Truck.NameMustBeUnique);

        return Result<int>.Success();
    }
}