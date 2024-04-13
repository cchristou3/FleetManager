using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Features.Trucks.DTOs;
using Fleet.Api.Features.Trucks.Implementations;
using Fleet.Api.Infrastructure;
using Fleet.Api.Shared.Requests;
using Fleet.Api.Testing.Extensions;
using Moq;

namespace Fleet.Api.Testing;

public class TruckContainerServiceTests
{
    private readonly Mock<IContainerRepository> _containerRepository;
    private readonly Mock<IShipContainerRepository> _shipContainerRepository;
    private readonly Mock<ITruckContainerRepository> _truckContainerRepository;
    private readonly Mock<ITruckRepository> _truckRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    public TruckContainerServiceTests()
    {
        _truckContainerRepository = new Mock<ITruckContainerRepository>();
        _containerRepository = new Mock<IContainerRepository>();
        _truckRepository = new Mock<ITruckRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _shipContainerRepository = new Mock<IShipContainerRepository>();
    }

    private ITruckContainerService GetTruckContainerService()
    {
        return new TruckContainerService(_truckRepository.Object, _unitOfWork.Object, _containerRepository.Object,
            _truckContainerRepository.Object, _shipContainerRepository.Object);
    }

    #region Load

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenTruckIsNotFound()
    {
        // Arrange
        var truckId = 1;
        var request = new LoadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Load(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.NotFound);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenTruckIsFull()
    {
        // Arrange
        var truckId = 1;
        var request = new LoadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Name = "Test", MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 2);

        // Act
        var result = await service.Load(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.IsFull);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenContainerIsNotFound()
    {
        // Arrange
        var truckId = 1;
        var request = new LoadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Name = "Test", MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Load(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NotFound);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenContainerIsAlreadyLoadedToThisTruck()
    {
        // Arrange
        var truckId = 1;
        var request = new LoadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Name = "Test", MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = truckId });

        // Act
        var result = await service.Load(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.AlreadyInDestination);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenContainerIsAlreadyLoadedToAnotherTruck()
    {
        // Arrange
        var truckId = 1;
        var request = new LoadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Name = "Test", MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer());

        // Act
        var result = await service.Load(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.LoadedInAnotherTruck);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenContainerIsAlreadyLoadedToShip()
    {
        // Arrange
        var truckId = 1;
        var request = new LoadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Name = "Test", MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer());

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer());

        // Act
        var result = await service.Load(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.LoadedInAnotherTruck);
    }

    [Fact]
    public async Task Load_ShouldReturnSuccessResult_WhenTruckExistsAndIsNotFullContainerExistsAndIsNotLoaded()
    {
        // Arrange
        var truckId = 1;
        var request = new LoadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Name = "Test", MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Load(truckId, request, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion

    #region Unload

    [Fact]
    public async Task Unload_ShouldReturnFailureResult_WhenContainerIsNotLoaded()
    {
        // Arrange
        var truckId = 1;
        var request = new UnloadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Unload(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NotLoaded);
    }

    [Fact]
    public async Task Unload_ShouldReturnFailureResult_WhenContainerIsLoadedInAnotherTruck()
    {
        // Arrange
        var truckId = 1;
        var request = new UnloadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = 2 });

        // Act
        var result = await service.Unload(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.LoadedInAnotherTruck);
    }

    [Fact]
    public async Task Unload_ShouldReturnSuccessResult_WhenContainerExistsAndIsLoadedToProvidedTruck()
    {
        // Arrange
        var truckId = 1;
        var request = new UnloadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = 1 });

        _truckContainerRepository.Setup(x => x.GetLatestLoaded(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Unload(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.IsEmpty);
    }

    [Fact]
    public async Task Unload_ShouldReturnFailureResult_WhenContainerIsNotTheLatestLoadedOne()
    {
        // Arrange
        var truckId = 1;
        var request = new UnloadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = 1, ContainerId = 1 });

        _truckContainerRepository.Setup(x => x.GetLatestLoaded(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = 1, ContainerId = 2 });

        // Act
        var result = await service.Unload(truckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.NotLatestLoaded);
    }

    [Fact]
    public async Task Unload_ShouldReturnSuccessResult_WhenContainerIsTheLatestLoadedOne()
    {
        // Arrange
        var truckId = 1;
        var request = new UnloadTruckRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = 1, ContainerId = 1 });

        _truckContainerRepository.Setup(x => x.GetLatestLoaded(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = 1, ContainerId = 1 });

        // Act
        var result = await service.Unload(truckId, request, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion

    #region Transfer

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenDestinationTruckIsTheSameAsSourceTruck()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 1;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.DestinationTruckCannotBeSameAsSourceTruck);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenSourceTruckIsNotFound()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.SourceTruckNotFound);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenDestinationTruckIsNotFound()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                sourceTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck());

        _truckRepository.Setup(x => x.Get(
                destinationTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.DestinationTruckNotFound);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenTruckContainerIsNotFound()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                sourceTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck());

        _truckRepository.Setup(x => x.Get(
                destinationTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck());

        _truckContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NotFound);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenTruckContainerIsNotLoadedInSourceTruck()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                sourceTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = sourceTruckId });

        _truckRepository.Setup(x => x.Get(
                destinationTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = destinationTruckId });

        _truckContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = 10 });

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.NotInSource);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenTruckContainerIsAlreadyLoadedInDestinationTruck()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                sourceTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = sourceTruckId });

        _truckRepository.Setup(x => x.Get(
                destinationTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = destinationTruckId });

        _truckContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = destinationTruckId });

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.AlreadyInDestination);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenDestinationTruckIsFull()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                sourceTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = sourceTruckId });

        _truckRepository.Setup(x => x.Get(
                destinationTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = destinationTruckId, MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = sourceTruckId });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 2);

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Truck.DestinationTruckIsFull);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenNotTransferringTheLatestContainer()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                sourceTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = sourceTruckId });

        _truckRepository.Setup(x => x.Get(
                destinationTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = destinationTruckId, MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = sourceTruckId });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _truckContainerRepository.Setup(x => x.GetLatestLoaded(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { ContainerId = 2 });

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.TruckContainer.NotLatestLoaded);
    }

    [Fact]
    public async Task
        Transfer_ShouldReturnSuccessResult_WhenBothTrucksExistAndSourceTruckHasTheContianerAndDestinationTruckIsNotFull()
    {
        // Arrange
        var sourceTruckId = 1;
        var destinationTruckId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetTruckContainerService();

        _truckRepository.Setup(x => x.Get(
                sourceTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = sourceTruckId });

        _truckRepository.Setup(x => x.Get(
                destinationTruckId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Truck { Id = destinationTruckId, MaximumCapacity = 2 });

        _truckContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { TruckId = sourceTruckId });

        _truckContainerRepository.Setup(x => x.CountContainersByTruckId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _truckContainerRepository.Setup(x => x.GetLatestLoaded(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer { ContainerId = 1 });

        // Act
        var result = await service.Transfer(sourceTruckId, destinationTruckId, request, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion
}