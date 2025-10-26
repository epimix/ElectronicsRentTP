using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class asdasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Video Equipment");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Audio Equipment");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Computers");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Laptops");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Microphones");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Cameras");

            migrationBuilder.InsertData(
                table: "EquipmentCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 7, "Projectors" },
                    { 8, "Screens" },
                    { 9, "Speakers" },
                    { 10, "Other" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Power Tools");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Construction Equipment");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Garden Tools");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Cleaning Equipment");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Electronics");

            migrationBuilder.UpdateData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Automotive");
        }
    }
}
