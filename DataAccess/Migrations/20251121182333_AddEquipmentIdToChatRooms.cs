using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentIdToChatRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Rentals_RentalId",
                table: "ChatRooms");

            migrationBuilder.RenameColumn(
                name: "RentalId",
                table: "ChatRooms",
                newName: "EquipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRooms_RentalId",
                table: "ChatRooms",
                newName: "IX_ChatRooms_EquipmentId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "551b73c1-3601-490c-90bf-5af17a4408d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0d26f82-6047-432b-b801-620e70979b7d", "AQAAAAIAAYagAAAAEPMSgQElR5arqFtEFOQ1cuJaYttKDsg1wKvYFKjkgpCm3znCodBoufpDwlMrRFZzGQ==", "5e54537a-9bb3-432e-aa9d-7a35be6bef6e" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Equipments_EquipmentId",
                table: "ChatRooms",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Equipments_EquipmentId",
                table: "ChatRooms");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "ChatRooms",
                newName: "RentalId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRooms_EquipmentId",
                table: "ChatRooms",
                newName: "IX_ChatRooms_RentalId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "551b73c1-3601-490c-90bf-5af17a4408d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "81a45d3d-62ae-4ff4-915c-a2cccfc43637", "AQAAAAIAAYagAAAAEKBEjgyp0xz2vQ+qEToIsRw4rUz7DKUA9FoRedUICK6qyvLKkFhldelBHepQd1FH1w==", "bba7e6b9-f5b5-4fd7-ac74-f92041083249" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Rentals_RentalId",
                table: "ChatRooms",
                column: "RentalId",
                principalTable: "Rentals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
