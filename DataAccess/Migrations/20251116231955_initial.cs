using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    profilePicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    ReviewSum = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "EquipmentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartEntities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartEntities_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Maintenances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maintenances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maintenances_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rentals_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EquipmentCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Video Equipment" },
                    { 2, "Audio Equipment" },
                    { 3, "Computers" },
                    { 4, "Laptops" },
                    { 5, "Microphones" },
                    { 6, "Cameras" },
                    { 7, "Projectors" },
                    { 8, "Screens" },
                    { 9, "Speakers" },
                    { 10, "Other" }
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "AverageRating", "CategoryId", "Description", "ImageUrl", "IsAvailable", "Name", "OwnerId", "PricePerHour", "Quantity", "ReviewCount", "ReviewSum", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, 0m, 6, "Professional mirrorless camera with 8K video recording and 45MP full-frame sensor", "https://cdn.media.amplience.net/i/canon/eos-r5_front_rf24-105mmf4lisusm_square_32c26ad194234d42b3cd9e582a21c99b", true, "Canon EOS R5 Camera", null, 50m, 2, 0, 0, 0, null },
                    { 2, 0m, 5, "High-quality wireless microphone for professional audio recording", "https://sony.scene7.com/is/image/sonyglobalsolutions/ULTMIC1_Intro2_M?$productIntroPlatemobile$&fmt=png-alpha", true, "Sony Wireless Microphone", null, 15m, 5, 0, 0, 0, null },
                    { 3, 0m, 4, "Powerful laptop for video editing and content creation", "https://bigmag.ua/image/cache/catalog/image/Product/Apple_MacBook_BY/Apple%20MacBook%20Pro%2016%20Space%20Gray%202019/Apple%20MacBook%20Pro%2016%20Space%20Gray%202019%201(1)-2000x2000.jpg", true, "MacBook Pro 16", null, 30m, 3, 0, 0, 0, null },
                    { 4, 0m, 6, "Professional drone with 4K camera and obstacle avoidance", "https://images.unsplash.com/photo-1473968512647-3e447244af8f?w=500", true, "DJI Mini 4 Pro Drone", null, 40m, 1, 0, 0, 0, null },
                    { 5, 0m, 9, "High-quality PA speakers for events and presentations", "https://thumbs.static-thomann.de/thumb/padthumb600x600/pics/bdb/_51/519347/16724814_800.jpg", true, "Bose Professional Speakers", null, 25m, 2, 0, 0, 0, null },
                    { 6, 0m, 7, "Professional 4K projector with 5000 lumens brightness", "https://musicmag.com.ua/media/catalog/product/cache/1/image/736x460/62defc7f46f3fbfc8afcd112227d1181/e/p/epson_pro_cinema_4040_front.jpg", true, "Epson Projector 4K", null, 35m, 2, 0, 0, 0, null },
                    { 7, 0m, 6, "Full-frame cinema camera for professional filmmaking with 4K 120fps recording", "https://fotomost.com.ua/content/images/28/500x500l50nn0/sony-fx3-56165663061752.jpg", true, "Sony FX3 Cinema Camera", null, 75m, 2, 0, 0, 0, null },
                    { 8, 0m, 5, "Broadcast-quality dynamic microphone perfect for podcasting and recording", "https://soundstore.com.ua/content/images/32/1200x800l80nn0/mikrofony-i-mikrofonnye-radiosistemy8318-shure-sm7b.html-40795060250278.jpg", true, "Shure SM7B Microphone", null, 20m, 4, 0, 0, 0, null },
                    { 9, 0m, 10, "A portable power station that can charge your devices and power your home.", "https://fotosale.ua/images/products/66/products.66193.1.b.jpg", true, "EcoFlow DELTA Max 2000 charging station", null, 25m, 3, 0, 0, 0, null },
                    { 10, 0m, 6, "Action camera with 5.3K video and HyperSmooth 6.0 stabilization", "https://photorent.kiev.ua/wp-content/uploads/GoPro-12-black-3.jpg", true, "GoPro Hero 12", null, 18m, 5, 0, 0, 0, null },
                    { 11, 0m, 2, "16-channel professional audio mixer with USB interface and effects", "https://www.hytekelectronics.co.uk/wp-content/uploads/2017/04/YAM-MG16XU.jpg", true, "Yamaha MG16XU Mixer", null, 30m, 2, 0, 0, 0, null },
                    { 12, 0m, 8, "Ultra HD 4K monitor perfect for video editing and color grading", "https://m.media-amazon.com/images/I/51bwiYTxx2L.jpg", true, "BenQ 4K Monitor 32 inch", null, 22m, 4, 0, 0, 0, null },
                    { 13, 0m, 5, "Professional on-camera shotgun microphone with advanced features", "https://prodj.ua/image/cache/catalog/img1b/2020/01/20200105081539-920x920.webp", true, "Rode VideoMic Pro+", null, 12m, 6, 0, 0, 0, null },
                    { 14, 0m, 6, "Professional zoom lens with constant f/2.8 aperture", "https://fotosale.ua/images/products/54/products.54671.1.b.jpg", true, "Canon RF 24-70mm f/2.8 Lens", null, 35m, 2, 0, 0, 0, null },
                    { 15, 0m, 2, "6-track portable audio recorder with interchangeable capsules", "https://prodj.ua/image/cache/catalog/img3b/2020/11/20201103133205-920x920.webp", true, "Zoom H6 Recorder", null, 28m, 3, 0, 0, 0, null },
                    { 16, 0m, 6, "The polarizing light filter increases the visual sharpness and purity of color in the photograph", "https://fotosale.ua/images/products/19/products.19876.1.b.jpg", true, "RODENSTOCK HR Digital Super MC Circular-Pol filter ", null, 45m, 2, 0, 0, 0, null },
                    { 17, 0m, 2, "Professional studio monitor headphones with exceptional sound quality", "https://fotosale.ua/images/products/66/products.66114.1.b.jpg", true, "Headphones Sennheiser RS 195", null, 10m, 8, 0, 0, 0, null },
                    { 18, 0m, 6, "Heavy-duty carbon fiber tripod with fluid head for smooth camera movements", "https://fotosale.ua/images/products/36/products.36866.1.b.jpg", true, "Manfrotto Tripod", null, 15m, 5, 0, 0, 0, null },
                    { 19, 0m, 3, "32-key customizable control deck for streaming and content creation", "https://res.cloudinary.com/elgato-pwa/image/upload/q_auto,f_auto/v1725280007/Products/10GBO9901%20%28Stream%20Deck%20Studio%29/ATF/Stream-Deck-Studio-ATF-04.jpg", true, "Elgato Stream Deck Studio", null, 18m, 4, 0, 0, 0, null },
                    { 20, 0m, 1, "Compact RGB constant light nameplate panel.", "https://fotosale.ua/images/products/67/products.67303.1.b.jpg", true, "Aputure amaran Ace 25c", null, 20m, 3, 0, 0, 0, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CartEntities_EquipmentId",
                table: "CartEntities",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CartEntities_UserId",
                table: "CartEntities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_CategoryId",
                table: "Equipments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_OwnerId",
                table: "Equipments",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_UserId",
                table: "Equipments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_EquipmentId",
                table: "Maintenances",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_EquipmentId",
                table: "Rentals",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_UserId",
                table: "Rentals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EquipmentId",
                table: "Reviews",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CartEntities");

            migrationBuilder.DropTable(
                name: "Maintenances");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EquipmentCategories");
        }
    }
}
