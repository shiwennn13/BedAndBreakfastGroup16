using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BedAndBreakfastGroup16.Migrations
{
    /// <inheritdoc />
    public partial class adduserrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userrole",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userrole",
                table: "AspNetUsers");
        }
    }
}
