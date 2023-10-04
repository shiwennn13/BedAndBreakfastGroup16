using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BedAndBreakfastGroup16.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomBedandServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bed",
                table: "RoomsTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "RoomsTable",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bed",
                table: "RoomsTable");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "RoomsTable");
        }
    }
}
