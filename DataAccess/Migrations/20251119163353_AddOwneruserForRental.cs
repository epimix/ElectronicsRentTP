using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddOwneruserForRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Rentals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_OwnerId",
                table: "Rentals",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_AspNetUsers_OwnerId",
                table: "Rentals",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_AspNetUsers_OwnerId",
                table: "Rentals");

            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_OwnerId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Rentals");

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_AspNetUsers_UserId",
                table: "Rentals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
