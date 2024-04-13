using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fleet.Api.Migrations
{
    /// <inheritdoc />
    public partial class Rename_To_MaxiumCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Trucks",
                newName: "MaximumCapacity");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Ships",
                newName: "MaximumCapacity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaximumCapacity",
                table: "Trucks",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "MaximumCapacity",
                table: "Ships",
                newName: "Capacity");
        }
    }
}
