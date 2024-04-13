using System.Collections.Generic;

namespace Fleet.Api.Features.Trucks.DTOs;

public class GetTruckResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int MaximumCapacity { get; set; }

    public int CurrentCapacity { get; set; }

    public ICollection<GetContainerResponse> LoadedContainers { get; set; }
}