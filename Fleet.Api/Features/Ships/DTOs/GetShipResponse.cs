using System.Collections.Generic;
using Fleet.Api.Features.Containers.DTOs;

namespace Fleet.Api.Features.Ships.DTOs;

public class GetShipResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int MaximumCapacity { get; set; }

    public int CurrentCapacity { get; set; }

    public ICollection<GetContainerResponse> LoadedContainers { get; set; }
}