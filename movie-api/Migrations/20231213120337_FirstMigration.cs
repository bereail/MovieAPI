using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie_api.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    lastname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    pass = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    rol = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employee_num = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdUserNavigationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Users_IdUserNavigationId",
                        column: x => x.IdUserNavigationId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "date", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "date", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IdUserNavigationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_IdUserNavigationId",
                        column: x => x.IdUserNavigationId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdUserNavigationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Users_IdUserNavigationId",
                        column: x => x.IdUserNavigationId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    director = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdAdmin = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdAdminNavigationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Admins_IdAdminNavigationId",
                        column: x => x.IdAdminNavigationId,
                        principalTable: "Admins",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BookingDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDate = table.Column<DateTime>(type: "date", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdBooking = table.Column<int>(type: "int", nullable: false),
                    IdMovie = table.Column<int>(type: "int", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "date", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    IdBookingNavigationId = table.Column<int>(type: "int", nullable: true),
                    IdMovieNavigationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingDetails_Bookings_IdBookingNavigationId",
                        column: x => x.IdBookingNavigationId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BookingDetails_Movies_IdMovieNavigationId",
                        column: x => x.IdMovieNavigationId,
                        principalTable: "Movies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_IdUserNavigationId",
                table: "Admins",
                column: "IdUserNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_IdBookingNavigationId",
                table: "BookingDetails",
                column: "IdBookingNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_IdMovieNavigationId",
                table: "BookingDetails",
                column: "IdMovieNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_IdUserNavigationId",
                table: "Bookings",
                column: "IdUserNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IdUserNavigationId",
                table: "Clients",
                column: "IdUserNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_IdAdminNavigationId",
                table: "Movies",
                column: "IdAdminNavigationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingDetails");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
