using Fleet.Api.Entities;

namespace Fleet.Api.Features.Ships.DTOs;

public class OnShipUnloadedEvent
{
    public int ShipId { get; set; }
    public UnloadedContainer UnloadedContainer { get; set; }

    public OnShipUnloadedEvent(int shipId, Container container)
    {
        ShipId = shipId;
        UnloadedContainer = new UnloadedContainer(container);
    } 
}

public class UnloadedContainer
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    
    public UnloadedContainer(Container container)
    {
        Id = container.Id;
        Name = container.Name;
    } 
}