using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class nullchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdminComment",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "551b73c1-3601-490c-90bf-5af17a4408d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "53e9d7cf-3d8e-4dfb-84d1-d945365cda0a", "AQAAAAIAAYagAAAAEPTTXzD3IVldtyL7DBVqQUJw9/KCrGQUBBPBXt81fdxEqsdmeM/9/AsvobsTDKI6EA==", "fd99b523-e258-4951-bde1-5f0e99a55e30" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdminComment",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "551b73c1-3601-490c-90bf-5af17a4408d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ceb84091-8809-4928-8d24-29af6c532400", "AQAAAAIAAYagAAAAEEeTkSOpuIRHbNFYXEcYvFp/mwO4Fz1eODBpA3rtEVtlEDmBHGf7tCjZRFQemtqtIg==", "8f9cbf57-307d-4973-b63f-40fa463d9528" });
        }
    }
}
