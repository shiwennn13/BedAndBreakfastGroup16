using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BedAndBreakfastGroup16.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingInformationTable",
                columns: table => new
                {
                    BookInfoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomType = table.Column<int>(type: "int", nullable: false),
                    RoomPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BookingCustName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookingContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookingContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    pax = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingInformationTable", x => x.BookInfoID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingInformationTable");
        }
    }
}
