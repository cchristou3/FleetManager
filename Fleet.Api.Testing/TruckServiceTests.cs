using AutoMapper;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Features.Trucks.DTOs;
using Fleet.Api.Features.Trucks.Implementations;
using Fleet.Api.Infrastructure;
using Fleet.Api.Testing.Extensions;
using FluentAssertions;
using Moq;

namespace Fleet.Api.Testing;

public class TruckServiceTests
{
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ITruckRepository> _truckRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    public TruckServiceTests()
    {
        _truckRepository = new Mock<ITruckRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
    }

    private ITruckService GetTruckService()
    {
        return new TruckService(_truckRepository.Object, _unitOfWork.Object, _mapper.Object);
    }

    #region Create

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsNotUnique()
    {
        // Arrange
        var request = new CreateTruckRequest { Name = "Bamboos Truck", Capacity = 1 };
        var service = GetTruckService();

        _truckRepository.Setup(x => x.IsNameUnique(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.NameMustBeUnique);
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenCapacityIsLargerThanMaximumAllowedCapacity()
    {
        // Arrange
        var request = new CreateTruckRequest
            { Name = "Bamboos Truck", Capacity = TruckService.TruckMaximumCapacity + 1 };
        var service = GetTruckService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.CapacityOutOfBounds(TruckService.TruckMaximumCapacity));
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenCapacityNegative()
    {
        // Arrange
        var request = new CreateTruckRequest { Name = "Bamboos Truck", Capacity = -1 };
        var service = GetTruckService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.CapacityOutOfBounds(TruckService.TruckMaximumCapacity));
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenCapacityIsZero()
    {
        // Arrange
        var request = new CreateTruckRequest { Name = "Bamboos Truck", Capacity = 0 };
        var service = GetTruckService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.CapacityOutOfBounds(TruckService.TruckMaximumCapacity));
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateTruckRequest { Name = string.Empty };
        var service = GetTruckService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.NameCannotBeEmpty);
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsNull()
    {
        // Arrange
        var request = new CreateTruckRequest { Name = null };
        var service = GetTruckService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.NameCannotBeEmpty);
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameExceedsMaximumAllowedCharacters()
    {
        // Arrange
        var request = new CreateTruckRequest
        {
            Name =
                "Hello This is A Random Hello World.Hello This is A Random Hello World.Hello This is A Random Hello World."
        };
        var service = GetTruckService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.TooLong(TruckService.TruckNameMaximumLength));
    }

    [Fact]
    public async Task Create_ShouldReturnSuccessResult_WhenNameIsUniqueAndNotEmptyOrNull()
    {
        // Arrange
        var request = new CreateTruckRequest { Name = "Bamboos Truck", Capacity = 1 };
        var service = GetTruckService();

        _truckRepository.Setup(x => x.IsNameUnique(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion

    #region Get

    [Fact]
    public async Task Get_ShouldReturnFailureResult_WhenTruckDoesNotExist()
    {
        // Arrange
        var truckId = 1;
        var service = GetTruckService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Get(truckId, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.NotFound);
    }

    [Fact]
    public async Task Get_ShouldReturnSuccessResult_WhenTruckExists()
    {
        // Arrange
        var truckId = 1;
        var service = GetTruckService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck
            {
                TruckContainers = new List<TruckContainer>()
            });

        _mapper.Setup(x => x.Map<Truck, GetTruckResponse>(It.IsAny<Truck>()))
            .Returns(() => new GetTruckResponse());

        // Act
        var result = await service.Get(truckId, default);

        // Assert
        result.ShouldBeSuccess();
        result.Value.Should().NotBe(null);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_ShouldReturnFailureResult_WhenTruckIsNotFound()
    {
        // Arrange
        var truckId = 1;
        var service = GetTruckService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Delete(truckId, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnSuccessResult_WhenTruckExists()
    {
        // Arrange
        var truckId = 1;
        var service = GetTruckService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck());

        // Act
        var result = await service.Delete(truckId, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion
}