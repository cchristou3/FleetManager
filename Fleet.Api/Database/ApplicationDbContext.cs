using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;
using Fleet.Api.Features.Containers.Implementations;
using Fleet.Api.Features.Ships.Implementations;
using Fleet.Api.Features.Trucks.Implementations;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Container> Containers { get; set; }

    public DbSet<Ship> Ships { get; set; }

    public DbSet<ShipContainer> ShipContainers { get; set; }

    public DbSet<Truck> Trucks { get; set; }

    public DbSet<TruckContainer> TruckContainers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Container>()
            .Property(p => p.Name)
            .HasMaxLength(ContainerService.ContainerNameMaximumLength)
            .IsRequired();

        modelBuilder.Entity<Ship>()
            .Property(p => p.MaximumCapacity)
            .HasDefaultValue(ShipService.ShipMaximumCapacity)
            .IsRequired();

        // A ship may have multiple ship containers
        // but a ship container may belong to only one ship
        modelBuilder.Entity<Ship>()
            .HasMany(e => e.ShipContainers)
            .WithOne(e => e.Ship)
            .HasForeignKey(e => e.ShipId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // A container may be a ship container
        // and a ship container must be a container
        modelBuilder.Entity<Container>()
            .HasOne(e => e.ShipContainer)
            .WithOne(e => e.Container)
            .HasForeignKey<ShipContainer>(e => e.ContainerId)
            .IsRequired(false);

        modelBuilder.Entity<Truck>()
            .Property(p => p.MaximumCapacity)
            .HasDefaultValue(TruckService.TruckMaximumCapacity)
            .IsRequired();

        // A truck may have multiple truck containers
        // but a truck container may belong to only one truck
        modelBuilder.Entity<Truck>()
            .HasMany(e => e.TruckContainers)
            .WithOne(e => e.Truck)
            .HasForeignKey(e => e.TruckId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // A container may be a truck container
        // and a truck container must be a container
        modelBuilder.Entity<Container>()
            .HasOne(e => e.TruckContainer)
            .WithOne(e => e.Container)
            .HasForeignKey<TruckContainer>(e => e.ContainerId)
            .IsRequired(false);

        modelBuilder.Entity<TruckContainer>()
            .Property(p => p.DateLoaded)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
    }

    public async Task<PaginationResult<TSource>> Paginate<TSource>(PaginationParams paginationParams,
        CancellationToken ct = default)
        where TSource : BaseEntity
    {
        var source = Set<TSource>();
        var count = await source.CountAsync(ct);
        var items = await source
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(ct);

        return new PaginationResult<TSource>(items, count);
    }
}