using Fleet.Api.Database;
using Fleet.Api.Features.Containers.Abstractions;
using Fleet.Api.Features.Containers.Implementations;
using Fleet.Api.Features.Ships.Abstractions;
using Fleet.Api.Features.Ships.Implementations;
using Fleet.Api.Features.Trucks.Abstractions;
using Fleet.Api.Features.Trucks.Implementations;
using Fleet.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShipService = Fleet.Api.Features.Ships.Implementations.ShipService;

namespace Fleet.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddSwaggerGen();

        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IContainerRepository, ContainerRepository>();
        services.AddScoped<IContainerService, ContainerService>();

        services.AddScoped<IShipRepository, ShipRepository>();
        services.AddScoped<IShipService, ShipService>();

        services.AddScoped<IShipContainerRepository, ShipContainerRepository>();
        services.AddScoped<IShipContainerService, ShipContainerService>();

        services.AddScoped<ITruckRepository, TruckRepository>();
        services.AddScoped<ITruckService, TruckService>();

        services.AddScoped<ITruckContainerRepository, TruckContainerRepository>();
        services.AddScoped<ITruckContainerService, TruckContainerService>();
    }
}