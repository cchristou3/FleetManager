using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Fleet.Api.Entities;
using Fleet.Api.Errors;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Features.Containers.DTOs;
using Fleet.Api.Features.Containers.Implementations;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Infrastructure;
using Fleet.Api.Testing.Extensions;
using FluentAssertions;
using Moq;

namespace Fleet.Api.Testing;

public class ContainerServiceTests
{
    private readonly Mock<IContainerRepository> _containerRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IShipContainerRepository> _shipContainerRepository;
    private readonly Mock<ITruckContainerRepository> _truckContainerRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;

    public ContainerServiceTests()
    {
        _shipContainerRepository = new Mock<IShipContainerRepository>();
        _truckContainerRepository = new Mock<ITruckContainerRepository>();
        _containerRepository = new Mock<IContainerRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
    }

    private IContainerService GetContainerService()
    {
        return new ContainerService(
            _containerRepository.Object, _shipContainerRepository.Object, _truckContainerRepository.Object,
            _unitOfWork.Object, _mapper.Object);
    }

    #region Create

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsNotUnique()
    {
        // Arrange
        var request = new CreateContainerRequest { Name = "Bamboos Container" };
        var service = GetContainerService();

        _containerRepository.Setup(x => x.IsNameUnique(
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NameMustBeUnique);
    }


    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateContainerRequest { Name = string.Empty };
        var service = GetContainerService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NameCannotBeEmpty);
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameIsNull()
    {
        // Arrange
        var request = new CreateContainerRequest { Name = null };
        var service = GetContainerService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NameCannotBeEmpty);
    }

    [Fact]
    public async Task Create_ShouldReturnFailureResult_WhenNameExceedsMaximumAllowedCharacters()
    {
        // Arrange
        var request = new CreateContainerRequest
        {
            Name =
                "string.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Emptystring.Empty"
        };
        var service = GetContainerService();

        // Act
        var result = await service.Create(request, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.TooLong(ContainerService.ContainerNameMaximumLength));
    }

    [Fact]
    public async Task Create_ShouldReturnSuccessResult_WhenNameIsUniqueAndNotEmptyOrNull()
    {
        // Arrange
        var request = new CreateContainerRequest { Name = "Bamboos Container" };
        var service = GetContainerService();

        _containerRepository.Setup(x => x.IsNameUnique(
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
    public async Task Get_ShouldReturnFailureResult_WhenContainerDoesNotExist()
    {
        // Arrange
        var containerId = 1;
        var service = GetContainerService();

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Get(containerId, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NotFound);
    }

    [Fact]
    public async Task Get_ShouldReturnSuccessResult_WhenContainerExists()
    {
        // Arrange
        var containerId = 1;
        var service = GetContainerService();

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        // Act
        var result = await service.Get(containerId, default);

        // Assert
        result.ShouldBeSuccess();
        result.Value.Should().NotBe(null);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_ShouldReturnFailureResult_WhenContainerIsNotFound()
    {
        // Arrange
        var containerId = 1;
        var service = GetContainerService();

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Delete(containerId, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnFailureResult_WhenLoadedInAShip()
    {
        // Arrange
        var containerId = 1;
        var service = GetContainerService();

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ShipContainer());

        // Act
        var result = await service.Delete(containerId, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.LoadedInShip);
    }

    [Fact]
    public async Task Delete_ShouldReturnFailureResult_WhenLoadedInATruck()
    {
        // Arrange
        var containerId = 1;
        var service = GetContainerService();

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new TruckContainer());

        // Act
        var result = await service.Delete(containerId, default);

        // Assert
        result.ShouldBeThisFailure(DomainErrors.Container.LoadedInTruck);
    }

    [Fact]
    public async Task Delete_ShouldReturnSuccessResult_WhenContainerExistsAndIsNotLoadedAnywhere()
    {
        // Arrange
        var containerId = 1;
        var service = GetContainerService();

        _containerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new Container());

        _shipContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        _truckContainerRepository.Setup(x => x.Get(
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await service.Delete(containerId, default);

        // Assert
        result.ShouldBeSuccess();
    }

    #endregion
}