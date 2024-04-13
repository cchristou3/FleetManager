﻿// <auto-generated />
using System;
using Fleet.Api.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Fleet.Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240228203124_Merge_Migrations")]
    partial class Merge_Migrations
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Fleet.Api.Entities.Container", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Containers");
                });

            modelBuilder.Entity("Fleet.Api.Entities.Ship", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(4);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ships");
                });

            modelBuilder.Entity("Fleet.Api.Entities.ShipContainer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ContainerId")
                        .HasColumnType("int");

                    b.Property<int>("ShipId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContainerId")
                        .IsUnique();

                    b.HasIndex("ShipId");

                    b.ToTable("ShipContainers");
                });

            modelBuilder.Entity("Fleet.Api.Entities.Truck", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(3);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Trucks");
                });

            modelBuilder.Entity("Fleet.Api.Entities.TruckContainer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ContainerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateLoaded")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("TruckId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContainerId")
                        .IsUnique();

                    b.HasIndex("TruckId");

                    b.ToTable("TruckContainers");
                });

            modelBuilder.Entity("Fleet.Api.Entities.ShipContainer", b =>
                {
                    b.HasOne("Fleet.Api.Entities.Container", "Container")
                        .WithOne("ShipContainer")
                        .HasForeignKey("Fleet.Api.Entities.ShipContainer", "ContainerId");

                    b.HasOne("Fleet.Api.Entities.Ship", "Ship")
                        .WithMany("ShipContainers")
                        .HasForeignKey("ShipId");

                    b.Navigation("Container");

                    b.Navigation("Ship");
                });

            modelBuilder.Entity("Fleet.Api.Entities.TruckContainer", b =>
                {
                    b.HasOne("Fleet.Api.Entities.Container", "Container")
                        .WithOne("TruckContainer")
                        .HasForeignKey("Fleet.Api.Entities.TruckContainer", "ContainerId");

                    b.HasOne("Fleet.Api.Entities.Truck", "Truck")
                        .WithMany("TruckContainers")
                        .HasForeignKey("TruckId");

                    b.Navigation("Container");

                    b.Navigation("Truck");
                });

            modelBuilder.Entity("Fleet.Api.Entities.Container", b =>
                {
                    b.Navigation("ShipContainer");

                    b.Navigation("TruckContainer");
                });

            modelBuilder.Entity("Fleet.Api.Entities.Ship", b =>
                {
                    b.Navigation("ShipContainers");
                });

            modelBuilder.Entity("Fleet.Api.Entities.Truck", b =>
                {
                    b.Navigation("TruckContainers");
                });
#pragma warning restore 612, 618
        }
    }
}
