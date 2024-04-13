using System;

namespace Fleet.Api.Features.Trucks.DTOs;

public class GetContainerResponse
{
    public int ContainerId { get; set; }
    public string Name { get; set; }
    public DateTime DateLoaded { get; set; }
}