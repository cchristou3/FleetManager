namespace Fleet.Api.Features.Trucks.DTOs;

public class CreateTruckRequest
{
    public string Name { get; set; }
    public int Capacity { get; set; }

    public bool IsCapacityOutOfBounds(int maximumCapacity)
    {
        return Capacity <= 0 || Capacity > maximumCapacity;
    }
}