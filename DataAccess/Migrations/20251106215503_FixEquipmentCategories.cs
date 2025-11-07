using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixEquipmentCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "AverageRating", "CategoryId", "Description", "ImageUrl", "IsAvailable", "Name", "PricePerHour", "Quantity", "ReviewCount", "ReviewSum", "Status", "UserId" },
                values: new object[,]
                {
                    { 7, 0m, 6, "Full-frame cinema camera for professional filmmaking with 4K 120fps recording", "https://fotomost.com.ua/content/images/28/500x500l50nn0/sony-fx3-56165663061752.jpg", true, "Sony FX3 Cinema Camera", 75m, 2, 0, 0, 0, null },
                    { 8, 0m, 5, "Broadcast-quality dynamic microphone perfect for podcasting and recording", "https://soundstore.com.ua/content/images/32/1200x800l80nn0/mikrofony-i-mikrofonnye-radiosistemy8318-shure-sm7b.html-40795060250278.jpg", true, "Shure SM7B Microphone", 20m, 4, 0, 0, 0, null },
                    { 9, 0m, 10, "A portable power station that can charge your devices and power your home.", "https://fotosale.ua/images/products/66/products.66193.1.b.jpg", true, "EcoFlow DELTA Max 2000 charging station", 25m, 3, 0, 0, 0, null },
                    { 10, 0m, 6, "Action camera with 5.3K video and HyperSmooth 6.0 stabilization", "https://photorent.kiev.ua/wp-content/uploads/GoPro-12-black-3.jpg", true, "GoPro Hero 12", 18m, 5, 0, 0, 0, null },
                    { 11, 0m, 2, "16-channel professional audio mixer with USB interface and effects", "https://www.hytekelectronics.co.uk/wp-content/uploads/2017/04/YAM-MG16XU.jpg", true, "Yamaha MG16XU Mixer", 30m, 2, 0, 0, 0, null },
                    { 12, 0m, 8, "Ultra HD 4K monitor perfect for video editing and color grading", "https://m.media-amazon.com/images/I/51bwiYTxx2L.jpg", true, "BenQ 4K Monitor 32 inch", 22m, 4, 0, 0, 0, null },
                    { 13, 0m, 5, "Professional on-camera shotgun microphone with advanced features", "https://prodj.ua/image/cache/catalog/img1b/2020/01/20200105081539-920x920.webp", true, "Rode VideoMic Pro+", 12m, 6, 0, 0, 0, null },
                    { 14, 0m, 6, "Professional zoom lens with constant f/2.8 aperture", "https://fotosale.ua/images/products/54/products.54671.1.b.jpg", true, "Canon RF 24-70mm f/2.8 Lens", 35m, 2, 0, 0, 0, null },
                    { 15, 0m, 2, "6-track portable audio recorder with interchangeable capsules", "https://prodj.ua/image/cache/catalog/img3b/2020/11/20201103133205-920x920.webp", true, "Zoom H6 Recorder", 28m, 3, 0, 0, 0, null },
                    { 16, 0m, 6, "The polarizing light filter increases the visual sharpness and purity of color in the photograph", "https://fotosale.ua/images/products/19/products.19876.1.b.jpg", true, "RODENSTOCK HR Digital Super MC Circular-Pol filter ", 45m, 2, 0, 0, 0, null },
                    { 17, 0m, 2, "Professional studio monitor headphones with exceptional sound quality", "https://fotosale.ua/images/products/66/products.66114.1.b.jpg", true, "Headphones Sennheiser RS 195", 10m, 8, 0, 0, 0, null },
                    { 18, 0m, 6, "Heavy-duty carbon fiber tripod with fluid head for smooth camera movements", "https://fotosale.ua/images/products/36/products.36866.1.b.jpg", true, "Manfrotto Tripod", 15m, 5, 0, 0, 0, null },
                    { 19, 0m, 3, "32-key customizable control deck for streaming and content creation", "https://res.cloudinary.com/elgato-pwa/image/upload/q_auto,f_auto/v1725280007/Products/10GBO9901%20%28Stream%20Deck%20Studio%29/ATF/Stream-Deck-Studio-ATF-04.jpg", true, "Elgato Stream Deck Studio", 18m, 4, 0, 0, 0, null },
                    { 20, 0m, 1, "Compact RGB constant light nameplate panel.", "https://fotosale.ua/images/products/67/products.67303.1.b.jpg", true, "Aputure amaran Ace 25c", 20m, 3, 0, 0, 0, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
