using System.Collections.Generic;
using Fleet.Api.Features.Trucks.DTOs;

namespace Fleet.Api.Entities;

public class Truck : BaseEntity
{
    public string Name { get; set; }

    public int MaximumCapacity { get; set; }

    public IReadOnlyCollection<TruckContainer> TruckContainers { get; set; }

    public bool IsFull(int numberOfTruckContainers)
    {
        return numberOfTruckContainers == MaximumCapacity;
    }

    public static Truck Create(CreateTruckRequest request)
    {
        return new Truck
        {
            Name = request.Name
        };
    }
}