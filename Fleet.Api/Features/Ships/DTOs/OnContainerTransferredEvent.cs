using Fleet.Api.Entities;

namespace Fleet.Api.Features.Ships.DTOs;

public class OnContainerTransferredEvent
{
    public int SourceShipId { get; set; }
    public int DestinationShipId  { get; set; }
    public SelectedContainer SelectedContainer { get; set; }

    public OnContainerTransferredEvent(int sourceShipId, int destinationShipId, Container container)
    {
        SourceShipId = sourceShipId;
        DestinationShipId = destinationShipId;
        SelectedContainer = new SelectedContainer(container);
    } 
}

public class SelectedContainer
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    
    public SelectedContainer(Container container)
    {
        Id = container.Id;
        Name = container.Name;
    } 
}