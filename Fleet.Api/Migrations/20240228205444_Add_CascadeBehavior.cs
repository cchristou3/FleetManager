using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fleet.Api.Migrations
{
    /// <inheritdoc />
    public partial class Add_CascadeBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipContainers_Ships_ShipId",
                table: "ShipContainers");

            migrationBuilder.DropForeignKey(
                name: "FK_TruckContainers_Trucks_TruckId",
                table: "TruckContainers");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipContainers_Ships_ShipId",
                table: "ShipContainers",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TruckContainers_Trucks_TruckId",
                table: "TruckContainers",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipContainers_Ships_ShipId",
                table: "ShipContainers");

            migrationBuilder.DropForeignKey(
                name: "FK_TruckContainers_Trucks_TruckId",
                table: "TruckContainers");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipContainers_Ships_ShipId",
                table: "ShipContainers",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TruckContainers_Trucks_TruckId",
                table: "TruckContainers",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id");
        }
    }
}
