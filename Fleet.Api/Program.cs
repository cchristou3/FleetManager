using System;
using System.Threading.Tasks;
using Fleet.Api.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fleet.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            Console.WriteLine("Migrations begin...");
            await using var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync(); // migrate database whenever we restart the application
            Console.WriteLine("======================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migrations failed: {ex}");
            throw;
        }
        
        Console.WriteLine("Server started working.");
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}