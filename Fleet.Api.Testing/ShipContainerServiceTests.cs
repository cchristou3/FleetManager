using AutoMapper;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Features.Real_time;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Ships.DTOs;
using Fleet.Api.Features.Ships.Implementations;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Features.Trucks.DTOs;
using Fleet.Api.Infrastructure;
using Fleet.Api.Shared.Requests;
using Fleet.Api.Testing.Extensions;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Fleet.Api.Testing;

public class ShipContainerServiceTests
{
    private readonly Mock<IContainerRepository> _containerRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IShipContainerRepository> _shipContainerRepository;
    private readonly Mock<IShipRepository> _shipRepository;
    private readonly Mock<ITruckContainerRepository> _truckContainerRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IHubContext<ShipHub>> _shipHub;

    public ShipContainerServiceTests()
    {
        _shipContainerRepository = new Mock<IShipContainerRepository>();
        _containerRepository = new Mock<IContainerRepository>();
        _shipRepository = new Mock<IShipRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _truckContainerRepository = new Mock<ITruckContainerRepository>();
        _shipHub = new Mock<IHubContext<ShipHub>>();
    }

    private IShipContainerService GetShipContainerService()
    {
        return new ShipContainerService(_shipRepository.Object, _unitOfWork.Object, _containerRepository.Object,
            _shipContainerRepository.Object, _truckContainerRepository.Object, _shipHub.Object);
    }

    #region Load

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenShipIsNotFound()
    {
        // Arrange
        var shipId = 1;
        var request = new LoadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Load(shipId, request, default, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.NotFound);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenShipIsFull()
    {
        // Arrange
        var shipId = 1;
        var request = new LoadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Name = "Test", MaximumCapacity = 2 });

        _shipContainerRepository.Setup(x => x.CountContainersByShipId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 2);

        // Act
        var result = await service.Load(shipId, request, default, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.IsFull);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenContainerIsNotFound()
    {
        // Arrange
        var shipId = 1;
        var request = new LoadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Name = "Test", MaximumCapacity = 2 });

        _shipContainerRepository.Setup(x => x.CountContainersByShipId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Load(shipId, request, default, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NotFound);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenContainerIsAlreadyLoadedToThisShip()
    {
        // Arrange
        var shipId = 1;
        var request = new LoadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Name = "Test", MaximumCapacity = 2 });

        _shipContainerRepository.Setup(x => x.CountContainersByShipId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer { ShipId = shipId });

        // Act
        var result = await service.Load(shipId, request, default, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.LoadedInShip);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenContainerIsAlreadyLoadedToAnotherShip()
    {
        // Arrange
        var shipId = 1;
        var request = new LoadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Name = "Test", MaximumCapacity = 2 });

        _shipContainerRepository.Setup(x => x.CountContainersByShipId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer());

        _mapper.Setup(x => x.Map<Truck, GetTruckResponse>(It.IsAny<Truck>()))
            .Returns(() => new GetTruckResponse());

        // Act
        var result = await service.Load(shipId, request, default, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.LoadedInShip);
    }

    [Fact]
    public async Task Load_ShouldReturnFailureResult_WhenContainerIsAlreadyLoadedToTruck()
    {
        // Arrange
        var shipId = 1;
        var request = new LoadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Name = "Test", MaximumCapacity = 2 });

        _shipContainerRepository.Setup(x => x.CountContainersByShipId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer());

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer());

        _mapper.Setup(x => x.Map<Truck, GetTruckResponse>(It.IsAny<Truck>()))
            .Returns(() => new GetTruckResponse());

        // Act
        var result = await service.Load(shipId, request, default, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.LoadedInTruck);
    }

    [Fact]
    public async Task Load_ShouldReturnSuccessResult_WhenShipExistsAndIsNotFullContainerExistsAndIsNotLoaded()
    {
        // Arrange
        var shipId = 1;
        var request = new LoadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Name = "Test", MaximumCapacity = 2 });

        _shipContainerRepository.Setup(x => x.CountContainersByShipId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        var clientProxy = new Mock<IClientProxy>();
        var clients = new Mock<IHubClients>();
    
        // We cannot mock extension methods so here we instead mock the method belonging
        // to the type that the extension method calls.
        clientProxy.Setup(x => 
            x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()));
        
        // Here we mock the underlying method as well.
        // E.g.,    from:   x.AllExcept(this IHubClients<T> hubClients, string excludedConnectionId)
        //          to:     hubClients.AllExcept(IReadOnlyList<string> excludedConnectionIds)
        clients.Setup(x => x.AllExcept(It.IsAny<string[]>()))
            .Returns(() => clientProxy.Object);
        
        _shipHub.Setup(x => x.Clients)
            .Returns(() => clients.Object);

        // Act
        var result = await service.Load(shipId, request, default, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion

    #region Unload

    [Fact]
    public async Task Unload_ShouldReturnFailureResult_WhenContainerIsNotLoaded()
    {
        // Arrange
        var shipId = 1;
        var request = new UnloadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Unload(shipId, request, default, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NotLoaded);
    }

    [Fact]
    public async Task Unload_ShouldReturnFailureResult_WhenContainerIsLoadedInAnotherShip()
    {
        // Arrange
        var shipId = 1;
        var request = new UnloadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer { ShipId = 2 });

        // Act
        var result = await service.Unload(shipId, request, default, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.ShipContainer.LoadedInAnotherShip);
    }

    [Fact]
    public async Task Unload_ShouldReturnSuccessResult_WhenContainerExistsAndIsLoadedToProvidedShip()
    {
        // Arrange
        var shipId = 1;
        var request = new UnloadShipRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer { ShipId = 1 });
        
        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container { Id = 1, Name = "Pambos"});
        
        var clientProxy = new Mock<IClientProxy>();
        var clients = new Mock<IHubClients>();
        
        // We cannot mock extension methods so here we instead mock the method belonging
        // to the type that the extension method calls.
        clientProxy.Setup(x => 
            x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()));
        
        // Here we mock the underlying method as well.
        // E.g.,    from:   x.AllExcept(this IHubClients<T> hubClients, string excludedConnectionId)
        //          to:     hubClients.AllExcept(IReadOnlyList<string> excludedConnectionIds)
        clients.Setup(x => x.AllExcept(It.IsAny<string[]>()))
            .Returns(() => clientProxy.Object);
        
        _shipHub.Setup(x => x.Clients)
            .Returns(() => clients.Object);

        // Act
        var result = await service.Unload(shipId, request, default, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion

    #region Transfer

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenDestinationShipIsTheSameAsSourceShip()
    {
        // Arrange
        var sourceShipId = 1;
        var destinationShipId = 1;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        // Act
        var result = await service.Transfer(sourceShipId, destinationShipId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.ShipContainer.DestinationShipCannotBeSameAsSourceShip);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenSourceShipIsNotFound()
    {
        // Arrange
        var sourceShipId = 1;
        var destinationShipId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Transfer(sourceShipId, destinationShipId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.SourceShipNotFound);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenDestinationShipIsNotFound()
    {
        // Arrange
        var sourceShipId = 1;
        var destinationShipId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                sourceShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship());

        _shipRepository.Setup(x => x.Get(
                destinationShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Transfer(sourceShipId, destinationShipId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.DestinationShipNotFound);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenShipContainerIsNotFound()
    {
        // Arrange
        var sourceShipId = 1;
        var destinationShipId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                sourceShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship());

        _shipRepository.Setup(x => x.Get(
                destinationShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship());

        _shipContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Transfer(sourceShipId, destinationShipId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NotFound);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenShipContainerIsNotLoadedInSourceShip()
    {
        // Arrange
        var sourceShipId = 1;
        var destinationShipId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                sourceShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Id = sourceShipId });

        _shipRepository.Setup(x => x.Get(
                destinationShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Id = destinationShipId });

        _shipContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer { ShipId = 10 });

        // Act
        var result = await service.Transfer(sourceShipId, destinationShipId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.ShipContainer.NotInSource);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenShipContainerIsAlreadyLoadedInDestinationShip()
    {
        // Arrange
        var sourceShipId = 1;
        var destinationShipId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                sourceShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Id = sourceShipId });

        _shipRepository.Setup(x => x.Get(
                destinationShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Id = destinationShipId });

        _shipContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer { ShipId = destinationShipId });

        // Act
        var result = await service.Transfer(sourceShipId, destinationShipId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.ShipContainer.AlreadyInDestination);
    }

    [Fact]
    public async Task Transfer_ShouldReturnFailureResult_WhenDestinationShipIsFull()
    {
        // Arrange
        var sourceShipId = 1;
        var destinationShipId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                sourceShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Id = sourceShipId });

        _shipRepository.Setup(x => x.Get(
                destinationShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Id = destinationShipId, MaximumCapacity = 2 });

        _shipContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer { ShipId = sourceShipId });

        _shipContainerRepository.Setup(x => x.CountContainersByShipId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 2);

        // Act
        var result = await service.Transfer(sourceShipId, destinationShipId, request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Ship.DestinationShipIsFull);
    }

    [Fact]
    public async Task
        Transfer_ShouldReturnSuccessResult_WhenBothShipsExistAndSourceShipHasTheContainerAndDestinationShipIsNotFull()
    {
        // Arrange
        var sourceShipId = 1;
        var destinationShipId = 2;
        var request = new TransferContainerRequest { ContainerId = 1 };
        var service = GetShipContainerService();

        _shipRepository.Setup(x => x.Get(
                sourceShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Id = sourceShipId });

        _shipRepository.Setup(x => x.Get(
                destinationShipId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Ship { Id = destinationShipId, MaximumCapacity = 2 });

        _shipContainerRepository.Setup(x => x.Get(
                request.ContainerId, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer { ShipId = sourceShipId });

        _shipContainerRepository.Setup(x => x.CountContainersByShipId(
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => 1);

        // Act
        var result = await service.Transfer(sourceShipId, destinationShipId, request, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion
}