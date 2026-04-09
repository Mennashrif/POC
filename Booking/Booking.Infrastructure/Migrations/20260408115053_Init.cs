using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StayDate_CheckIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StayDate_CheckOut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Guest_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guest_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guest_Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    _assignedPhysicalRoomIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomRequest",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRequest", x => new { x.ReservationId, x.Id });
                    table.ForeignKey(
                        name: "FK_RoomRequest_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomRequest");

            migrationBuilder.DropTable(
                name: "Reservations");
        }
    }
}
