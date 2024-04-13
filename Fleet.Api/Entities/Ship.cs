using System.Collections.Generic;
using Fleet.Api.Features.Ships.DTOs;

namespace Fleet.Api.Entities;

public class Ship : BaseEntity
{
    public string Name { get; set; }

    public int MaximumCapacity { get; set; }

    public IReadOnlyCollection<ShipContainer> ShipContainers { get; set; }

    public bool IsFull(int numberOfShipContainers)
    {
        return numberOfShipContainers == MaximumCapacity;
    }

    public static Ship Create(CreateShipRequest request)
    {
        return new Ship
        {
            Name = request.Name,
            MaximumCapacity = request.Capacity
        };
    }
}