using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BedAndBreakfastGroup16.Migrations
{
    /// <inheritdoc />
    public partial class addRoomTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userrole",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomsTable",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RoomSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomCapacity = table.Column<int>(type: "int", nullable: false),
                    RoomDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomsTable", x => x.RoomId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomsTable");

            migrationBuilder.DropColumn(
                name: "userrole",
                table: "AspNetUsers");
        }
    }
}
