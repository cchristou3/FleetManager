using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Ships.DTOs;
using Fleet.Api.Infrastructure;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;

namespace Fleet.Api.Features.Ships.Implementations;

/// <summary>
///     Service responsible for managing Ship entities and handling related operations.
/// </summary>
public class ShipService : IShipService
{
    /// <summary>
    ///     Maximum length allowed for a ship name.
    /// </summary>
    public const int ShipNameMaximumLength = 100;

    /// <summary>
    ///     Maximum number of containers allowed for a ship to hold.
    /// </summary>
    public const int ShipMaximumCapacity = 4;

    private readonly IMapper _mapper;
    private readonly IShipRepository _shipRepository;

    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    ///     Initializes a new instance of the ShipService class.
    /// </summary>
    /// <param name="shipRepository">Repository for ship data access.</param>
    /// <param name="unitOfWork">Unit of Work for managing transactions.</param>
    /// <param name="mapper">Mapper for DTO transformations.</param>
    public ShipService(IShipRepository shipRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _shipRepository = shipRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    ///     Creates a new ship based on the provided request.
    /// </summary>
    /// <param name="request">The request containing ship data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the create operation.</returns>
    public async Task<Result<int>> Create(CreateShipRequest request, CancellationToken ct = default)
    {
        var validationResult = await ValidateCreateRequest(request, ct);

        if (validationResult.HasFailed) return validationResult;

        var ship = Ship.Create(request);

        await _shipRepository.Create(ship, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(ship.Id);
    }

    /// <summary>
    ///     Gets details of a specific ship by its Id.
    /// </summary>
    /// <param name="shipId">The Id of the ship.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the ship details or an error if not found.</returns>
    public async Task<Result<GetShipResponse>> Get(int shipId, CancellationToken ct)
    {
        var ship = await _shipRepository.Get(shipId, false, ct);

        if (ship is null) return Result<GetShipResponse>.Failure(DomainErrors.Ship.NotFound);

        var response = _mapper.Map<Ship, GetShipResponse>(ship);
        return Result<GetShipResponse>.Success(response);
    }

    /// <summary>
    ///     Gets a paginated list of ships based on the provided parameters.
    /// </summary>
    /// <param name="paginationParams">Pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the paginated list of ships or an error if encountered.</returns>
    public async Task<Result<PaginationResponse<GetShipResponse>>> Get(PaginationParams paginationParams,
        CancellationToken ct)
    {
        var paginationResult = await _shipRepository.Get(paginationParams, ct);

        var transformedPagedItems = _mapper.Map<List<Ship>, List<GetShipResponse>>(paginationResult);

        var response =
            PaginationResponse<GetShipResponse>
                .Create(transformedPagedItems, paginationResult.TotalCount, paginationParams);

        return Result<PaginationResponse<GetShipResponse>>.Success(response);
    }

    /// <summary>
    ///     Deletes a ship by its Id.
    /// </summary>
    /// <param name="shipId">The Id of the ship to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the delete operation.</returns>
    public async Task<Result<object>> Delete(int shipId, CancellationToken ct)
    {
        var ship = await _shipRepository.Get(shipId, true, ct);

        if (ship is null) return Result<object>.Failure(DomainErrors.Ship.NotFound);

        _shipRepository.Remove(ship);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<object>.Success();
    }

    /// <summary>
    ///     Validates the create ship request.
    /// </summary>
    /// <param name="request">The request containing ship data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating the success or failure of the validation.</returns>
    private async Task<Result<int>> ValidateCreateRequest(CreateShipRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(request.Name))
            return Result<int>.Failure(DomainErrors.Ship.NameCannotBeEmpty);

        if (request.Name.Length > ShipNameMaximumLength)
            return Result<int>.Failure(DomainErrors.Ship.TooLong(ShipNameMaximumLength));

        if (request.IsCapacityOutOfBounds(ShipMaximumCapacity))
            return Result<int>.Failure(DomainErrors.Ship.CapacityOutOfBounds(ShipMaximumCapacity));

        if (!await _shipRepository.IsNameUnique(request.Name, ct))
            return Result<int>.Failure(DomainErrors.Ship.NameMustBeUnique);

        return Result<int>.Success();
    }
}