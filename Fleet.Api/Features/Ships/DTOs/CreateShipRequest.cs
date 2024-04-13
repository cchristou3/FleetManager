namespace Fleet.Api.Features.Ships.DTOs;

public class CreateShipRequest
{
    public string Name { get; set; }
    public int Capacity { get; set; }

    public bool IsCapacityOutOfBounds(int maximumCapacity)
    {
        return Capacity <= 0 || Capacity > maximumCapacity;
    }
}