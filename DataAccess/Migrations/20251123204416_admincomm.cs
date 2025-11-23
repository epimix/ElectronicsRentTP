using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class admincomm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "551b73c1-3601-490c-90bf-5af17a4408d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ceb84091-8809-4928-8d24-29af6c532400", "AQAAAAIAAYagAAAAEEeTkSOpuIRHbNFYXEcYvFp/mwO4Fz1eODBpA3rtEVtlEDmBHGf7tCjZRFQemtqtIg==", "8f9cbf57-307d-4973-b63f-40fa463d9528" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "Complaints");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "551b73c1-3601-490c-90bf-5af17a4408d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6f8fb3f9-2bac-441b-92c6-759522a9b74c", "AQAAAAIAAYagAAAAEGrim0nZkMw+CQKhwm2c8UaL+gGPrdMEW7bjh+0jRBgBtQqEV8vAGszWO1eGIRHImA==", "33aef106-e3f6-44c7-99d7-c11a91909082" });
        }
    }
}
