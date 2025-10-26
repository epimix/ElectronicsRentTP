using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class asdsds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://cdn.media.amplience.net/i/canon/eos-r5_front_rf24-105mmf4lisusm_square_32c26ad194234d42b3cd9e582a21c99b");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://sony.scene7.com/is/image/sonyglobalsolutions/ULTMIC1_Intro2_M?$productIntroPlatemobile$&fmt=png-alpha");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://bigmag.ua/image/cache/catalog/image/Product/Apple_MacBook_BY/Apple%20MacBook%20Pro%2016%20Space%20Gray%202019/Apple%20MacBook%20Pro%2016%20Space%20Gray%202019%201(1)-2000x2000.jpg");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://thumbs.static-thomann.de/thumb/padthumb600x600/pics/bdb/_51/519347/16724814_800.jpg");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://musicmag.com.ua/media/catalog/product/cache/1/image/736x460/62defc7f46f3fbfc8afcd112227d1181/e/p/epson_pro_cinema_4040_front.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1606983340126-99ab4feaa64a?w=500");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=500");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1541807084-5cc52b6b58f2?w=500");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1608043152269-0d6e1c6920b7?w=500");

            migrationBuilder.UpdateData(
                table: "Equipments",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1611339555311-eaa078c1319d?w=500");
        }
    }
}
