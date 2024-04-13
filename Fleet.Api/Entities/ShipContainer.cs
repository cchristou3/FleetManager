using Fleet.Api.Features.Ships.DTOs;

namespace Fleet.Api.Entities;

public class ShipContainer : BaseEntity
{
    public Ship Ship { get; set; }
    public int ShipId { get; set; }
    public Container Container { get; set; }
    public int ContainerId { get; set; }

    public static ShipContainer Create(int shipId, LoadShipRequest request)
    {
        return new ShipContainer
        {
            ShipId = shipId,
            ContainerId = request.ContainerId
        };
    }
}