using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChatPinDel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ChatRooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "ChatRooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "551b73c1-3601-490c-90bf-5af17a4408d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "069cd63b-0253-4254-9926-4ec08882513c", "AQAAAAIAAYagAAAAENGYpEP7ztDURJ0eVvmUkaIqJBBpe8Nh/GCybtHoR5AyY+8f6RISQciYqZ9QI5qUmw==", "93287861-cb30-4725-9490-46e51546a7db" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "ChatRooms");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "551b73c1-3601-490c-90bf-5af17a4408d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0d26f82-6047-432b-b801-620e70979b7d", "AQAAAAIAAYagAAAAEPMSgQElR5arqFtEFOQ1cuJaYttKDsg1wKvYFKjkgpCm3znCodBoufpDwlMrRFZzGQ==", "5e54537a-9bb3-432e-aa9d-7a35be6bef6e" });
        }
    }
}
