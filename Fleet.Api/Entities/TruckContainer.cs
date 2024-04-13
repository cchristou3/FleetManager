using System;
using Fleet.Api.Features.Trucks.DTOs;

namespace Fleet.Api.Entities;

public class TruckContainer : BaseEntity
{
    public Truck Truck { get; set; }
    public int TruckId { get; set; }
    public Container Container { get; set; }
    public int ContainerId { get; set; }

    public DateTime DateLoaded { get; set; }

    public static TruckContainer Create(int truckId, LoadTruckRequest request)
    {
        return new TruckContainer
        {
            TruckId = truckId,
            ContainerId = request.ContainerId,
            DateLoaded = DateTime.UtcNow
        };
    }
}