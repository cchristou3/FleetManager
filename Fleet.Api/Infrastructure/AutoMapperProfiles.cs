using System.Linq;
using AutoMapper;
using Fleet.Api.Entities;
using Fleet.Api.Features.Ships.DTOs;
using Fleet.Api.Features.Trucks.DTOs;
using GetContainerResponse = Fleet.Api.Features.Containers.DTOs.GetContainerResponse;

namespace Fleet.Api.Infrastructure;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Container, GetContainerResponse>();

        CreateMap<Ship, GetShipResponse>()
            .ForMember(dest => dest.CurrentCapacity,
                opt => opt
                    .MapFrom(src => src.ShipContainers.Count))
            .ForMember(dest => dest.LoadedContainers,
                opt => opt
                    .MapFrom(src => src
                        .ShipContainers
                        .Select(x => new GetContainerResponse
                        {
                            Id = x.Container.Id,
                            Name = x.Container.Name
                        })
                        .ToList()));

        CreateMap<Truck, GetTruckResponse>()
            .ForMember(dest => dest.CurrentCapacity,
                opt => opt
                    .MapFrom(src => src.TruckContainers.Count))
            .ForMember(dest => dest.LoadedContainers,
                opt => opt
                    .MapFrom(src => src
                        .TruckContainers
                        .Select(x => new Features.Trucks.DTOs.GetContainerResponse
                        {
                            ContainerId = x.Container.Id,
                            Name = x.Container.Name,
                            DateLoaded = x.DateLoaded.ToUniversalTime()
                        })
                        .ToList()));
    }
}