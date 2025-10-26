using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedEquipmentData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "IsAvailable", "Name", "PricePerHour", "Quantity" },
                values: new object[,]
                {
                    { 1, 6, "Professional mirrorless camera with 8K video recording and 45MP full-frame sensor", "https://images.unsplash.com/photo-1606983340126-99ab4feaa64a?w=500", true, "Canon EOS R5 Camera", 50m, 2 },
                    { 2, 5, "High-quality wireless microphone for professional audio recording", "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=500", true, "Sony Wireless Microphone", 15m, 5 },
                    { 3, 4, "Powerful laptop for video editing and content creation", "https://images.unsplash.com/photo-1541807084-5cc52b6b58f2?w=500", true, "MacBook Pro 16", 30m, 3 },
                    { 4, 6, "Professional drone with 4K camera and obstacle avoidance", "https://images.unsplash.com/photo-1473968512647-3e447244af8f?w=500", true, "DJI Mini 4 Pro Drone", 40m, 1 },
                    { 5, 9, "High-quality PA speakers for events and presentations", "https://images.unsplash.com/photo-1608043152269-0d6e1c6920b7?w=500", true, "Bose Professional Speakers", 25m, 2 },
                    { 6, 7, "Professional 4K projector with 5000 lumens brightness", "https://images.unsplash.com/photo-1611339555311-eaa078c1319d?w=500", true, "Epson Projector 4K", 35m, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
