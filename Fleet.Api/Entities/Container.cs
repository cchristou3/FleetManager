using Fleet.Api.Features.Containers.DTOs;

namespace Fleet.Api.Entities;

public class Container : BaseEntity
{
    public string Name { get; set; }

    public ShipContainer ShipContainer { get; set; }

    public TruckContainer TruckContainer { get; set; }

    public static Container Create(CreateContainerRequest request)
    {
        return new Container
        {
            Name = request.Name
        };
    }
}