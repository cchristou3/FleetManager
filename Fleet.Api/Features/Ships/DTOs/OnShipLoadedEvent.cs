using Fleet.Api.Entities;

namespace Fleet.Api.Features.Ships.DTOs;

public class OnShipLoadedEvent
{
    public int ShipId { get; set; }
    public LoadedContainer LoadedContainer { get; set; }

    public OnShipLoadedEvent(int shipId, Container container)
    {
        ShipId = shipId;
        LoadedContainer = new LoadedContainer(container);
    } 
}

public class LoadedContainer
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    
    public LoadedContainer(Container container)
    {
        Id = container.Id;
        Name = container.Name;
    } 
}