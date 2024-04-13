using System;
using System.IO;
using System.Reflection;
using Fleet.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Fleet.Api;

public class Startup(IWebHostEnvironment env)
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
            .Build();

        services.AddCors();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Fleet API", Version = "v1",
                Description =
                    "Fleet Management API Documentation\n\nThis API allows users to manage fleets of trucks, ships, and containers. The main entities include Trucks, Ships, and Containers, each having dedicated services for operations.\n\nTruckService:\n- Manages Truck entities.\n- Supports creating, retrieving, listing with pagination, and deleting trucks.\n- Provides methods to load and unload containers to/from trucks.\n\nShipsService:\n- Manages Ship entities.\n- Supports creating, retrieving, listing with pagination, and deleting ships.\n- Provides methods to load and unload containers to/from ships.\n\nContainerService:\n- Manages Container entities.\n- Supports creating, retrieving, listing with pagination, and deleting containers.\n- Ensures validation for container names, lengths, and uniqueness.\n- Prevents deletion of containers loaded in ships or trucks.\n\nCommon Features:\n- Utilizes AutoMapper for DTO transformations.\n- Implements unit of work pattern for managing transactions.\n- Supports asynchronous operations using CancellationToken.\n\nEndpoints:\n- TrucksController: /api/trucks\n- ShipsController: /api/ships\n- ContainersController: /api/containers\n\nFor detailed information on request and response models, refer to the below API documentation.\n"
            });

            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddControllers();
        services.AddApplicationServices(configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

        app.UseRouting();

        app.UseCors(x =>
            x.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins("http://localhost:4200"));

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
        });
    }
}