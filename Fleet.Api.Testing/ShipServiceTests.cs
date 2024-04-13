using AutoMapper;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Ships.DTOs;
using Fleet.Api.Features.Ships.Implementations;
using Fleet.Api.Infrastructure;
using Fleet.Api.Testing.Extensions;
using FluentAssertions;
using Moq;

namespace Fleet.Api.Testing;

public class ShipServiceTests
{
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IShipRepository> _shipRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    public ShipServiceTests()
    {
        _shipRepository = new Mock<IShipRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
    }

    private IShipService GetShipService()
    {
        return new ShipService(_shipRepository.Object, _unitOfWork.Object, _mapper.Object);
    }

    #region Create

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsNotUnique()
    {
        // Arrange
        var request = new CreateShipRequest { Name = "Bamboos Ship", Capacity = 1 };
        var service = GetShipService();

        _shipRepository.Setup(x => x.IsNameUnique(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.NameMustBeUnique);
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenCapacityIsLargerThanMaximumAllowedCapacity()
    {
        // Arrange
        var request = new CreateShipRequest { Name = "Bamboos Ship", Capacity = ShipService.ShipMaximumCapacity + 1 };
        var service = GetShipService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.CapacityOutOfBounds(ShipService.ShipMaximumCapacity));
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenCapacityNegative()
    {
        // Arrange
        var request = new CreateShipRequest { Name = "Bamboos Ship", Capacity = -1 };
        var service = GetShipService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.CapacityOutOfBounds(ShipService.ShipMaximumCapacity));
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenCapacityIsZero()
    {
        // Arrange
        var request = new CreateShipRequest { Name = "Bamboos Ship", Capacity = 0 };
        var service = GetShipService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.CapacityOutOfBounds(ShipService.ShipMaximumCapacity));
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateShipRequest { Name = string.Empty };
        var service = GetShipService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.NameCannotBeEmpty);
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsNull()
    {
        // Arrange
        var request = new CreateShipRequest { Name = null };
        var service = GetShipService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.NameCannotBeEmpty);
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameExceedsMaximumAllowedCharacters()
    {
        // Arrange
        var request = new CreateShipRequest
        {
            Name =
                "Hello This is A Random Hello World.Hello This is A Random Hello World.Hello This is A Random Hello World."
        };
        var service = GetShipService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.TooLong(ShipService.ShipNameMaximumLength));
    }

    [Fact]
    public async Task Create_ShouldReturnSuccessResult_WhenNameIsUniqueAndNotEmptyOrNull()
    {
        // Arrange
        var request = new CreateShipRequest { Name = "Bamboos Ship", Capacity = 1 };
        var service = GetShipService();

        _shipRepository.Setup(x => x.IsNameUnique(
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
    public async Task Get_ShouldReturnFailureResult_WhenShipDoesNotExist()
    {
        // Arrange
        var shipId = 1;
        var service = GetShipService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Get(shipId, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.NotFound);
    }

    [Fact]
    public async Task Get_ShouldReturnSuccessResult_WhenShipExists()
    {
        // Arrange
        var shipId = 1;
        var service = GetShipService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship());

        _mapper.Setup(x => x.Map<Ship, GetShipResponse>(It.IsAny<Ship>()))
            .Returns(() => new GetShipResponse());

        // Act
        var result = await service.Get(shipId, default);

        // Assert
        result.ShouldBeSuccess();
        result.Value.Should().NotBe(null);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_ShouldReturnFailureResult_WhenShipIsNotFound()
    {
        // Arrange
        var shipId = 1;
        var service = GetShipService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Delete(shipId, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnSuccessResult_WhenShipExists()
    {
        // Arrange
        var shipId = 1;
        var service = GetShipService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship());

        // Act
        var result = await service.Delete(shipId, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion
}