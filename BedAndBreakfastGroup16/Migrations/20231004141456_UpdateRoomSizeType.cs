using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BedAndBreakfastGroup16.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomSizeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userrole",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "RoomSize",
                table: "RoomsTable",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoomSize",
                table: "RoomsTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "userrole",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
