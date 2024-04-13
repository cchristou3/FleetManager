using Fleet.Api.Entities;

namespace Fleet.Api.Features.Containers.DTOs;

public class GetContainerResponse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public static GetContainerResponse Create(Container container)
    {
        return new GetContainerResponse
        {
            Id = container.Id,
            Name = container.Name
        };
    }
}