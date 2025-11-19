using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class EquipmentRentalDbContext : IdentityDbContext<User>
    {
        public EquipmentRentalDbContext(DbContextOptions<EquipmentRentalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentCategory> EquipmentCategories { get; set; }
 
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
        public DbSet<CartEntity> CartEntities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rental>()
              .HasOne(r => r.User)
              .WithMany(u => u.Rentals)
              .HasForeignKey(r => r.UserId)
              .OnDelete(DeleteBehavior.Restrict);

            // Owner of equipment
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Owner)
                .WithMany()
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal types
            modelBuilder.Entity<Rental>()
                .Property(r => r.TotalPrice)
                .HasColumnType("decimal(18,2)");


            // ----- EQUIPMENT -----

            modelBuilder.Entity<Equipment>()
                .Property(e => e.PricePerHour)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Equipment>()
                .Property(e => e.AverageRating)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Owner)
                .WithMany(u => u.MyAdverts)
                .HasForeignKey(e => e.OwnerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


            // ----- MAINTENANCE -----

            modelBuilder.Entity<Maintenance>()
                .Property(m => m.Cost)
                .HasColumnType("decimal(18,2)");


            // ----- USER -----

            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasColumnType("decimal(18,2)");

            // Seed initial categories
            modelBuilder.Entity<EquipmentCategory>().HasData(
                new EquipmentCategory { Id = 1, Name = "Video Equipment" },
                new EquipmentCategory { Id = 2, Name = "Audio Equipment" },
                new EquipmentCategory { Id = 3, Name = "Computers" },
                new EquipmentCategory { Id = 4, Name = "Laptops" },
                new EquipmentCategory { Id = 5, Name = "Microphones" },
                new EquipmentCategory { Id = 6, Name = "Cameras" },
                new EquipmentCategory { Id = 7, Name = "Projectors" },
                new EquipmentCategory { Id = 8, Name = "Screens" },
                new EquipmentCategory { Id = 9, Name = "Speakers" },
                new EquipmentCategory { Id = 10, Name = "Other" }
            );

            // Seed initial equipment
            modelBuilder.Entity<Equipment>().HasData(
                new Equipment
                {
                    Id = 1,
                    Name = "Canon EOS R5 Camera",
                    CategoryId = 6,
                    PricePerHour = 50,
                    Quantity = 2,
                    IsAvailable = true,
                    ImageUrl = "https://cdn.media.amplience.net/i/canon/eos-r5_front_rf24-105mmf4lisusm_square_32c26ad194234d42b3cd9e582a21c99b",
                    Description = "Professional mirrorless camera with 8K video recording and 45MP full-frame sensor"
                },
                new Equipment
                {
                    Id = 2,
                    Name = "Sony Wireless Microphone",
                    CategoryId = 5,
                    PricePerHour = 15,
                    Quantity = 5,
                    IsAvailable = true,
                    ImageUrl = "https://sony.scene7.com/is/image/sonyglobalsolutions/ULTMIC1_Intro2_M?$productIntroPlatemobile$&fmt=png-alpha",
                    Description = "High-quality wireless microphone for professional audio recording"
                },
                new Equipment
                {
                    Id = 3,
                    Name = "MacBook Pro 16",
                    CategoryId = 4,
                    PricePerHour = 30,
                    Quantity = 3,
                    IsAvailable = true,
                    ImageUrl = "https://bigmag.ua/image/cache/catalog/image/Product/Apple_MacBook_BY/Apple%20MacBook%20Pro%2016%20Space%20Gray%202019/Apple%20MacBook%20Pro%2016%20Space%20Gray%202019%201(1)-2000x2000.jpg",
                    Description = "Powerful laptop for video editing and content creation"
                },
                new Equipment
                {
                    Id = 4,
                    Name = "DJI Mini 4 Pro Drone",
                    CategoryId = 6,
                    PricePerHour = 40,
                    Quantity = 1,
                    IsAvailable = true,
                    ImageUrl = "https://images.unsplash.com/photo-1473968512647-3e447244af8f?w=500",
                    Description = "Professional drone with 4K camera and obstacle avoidance"
                },
                new Equipment
                {
                    Id = 5,
                    Name = "Bose Professional Speakers",
                    CategoryId = 9,
                    PricePerHour = 25,
                    Quantity = 2,
                    IsAvailable = true,
                    ImageUrl = "https://thumbs.static-thomann.de/thumb/padthumb600x600/pics/bdb/_51/519347/16724814_800.jpg",
                    Description = "High-quality PA speakers for events and presentations"
                },
                new Equipment
                {
                    Id = 6,
                    Name = "Epson Projector 4K",
                    CategoryId = 7,
                    PricePerHour = 35,
                    Quantity = 2,
                    IsAvailable = true,
                    ImageUrl = "https://musicmag.com.ua/media/catalog/product/cache/1/image/736x460/62defc7f46f3fbfc8afcd112227d1181/e/p/epson_pro_cinema_4040_front.jpg",
                    Description = "Professional 4K projector with 5000 lumens brightness"
                },
                new Equipment
                {
                    Id = 7,
                    Name = "Sony FX3 Cinema Camera",
                    CategoryId = 6,
                    PricePerHour = 75,
                    Quantity = 2,
                    IsAvailable = true,
                    ImageUrl = "https://fotomost.com.ua/content/images/28/500x500l50nn0/sony-fx3-56165663061752.jpg",
                    Description = "Full-frame cinema camera for professional filmmaking with 4K 120fps recording"
                },
                new Equipment
                {
                    Id = 8,
                    Name = "Shure SM7B Microphone",
                    CategoryId = 5,
                    PricePerHour = 20,
                    Quantity = 4,
                    IsAvailable = true,
                    ImageUrl = "https://soundstore.com.ua/content/images/32/1200x800l80nn0/mikrofony-i-mikrofonnye-radiosistemy8318-shure-sm7b.html-40795060250278.jpg",
                    Description = "Broadcast-quality dynamic microphone perfect for podcasting and recording"
                },
                new Equipment
                {
                    Id = 9,
                    Name = "EcoFlow DELTA Max 2000 charging station",
                    CategoryId = 10,
                    PricePerHour = 25,
                    Quantity = 3,
                    IsAvailable = true,
                    ImageUrl = "https://fotosale.ua/images/products/66/products.66193.1.b.jpg",
                    Description = "A portable power station that can charge your devices and power your home."
                },
                new Equipment
                {
                    Id = 10,
                    Name = "GoPro Hero 12",
                    CategoryId = 6,
                    PricePerHour = 18,
                    Quantity = 5,
                    IsAvailable = true,
                    ImageUrl = "https://photorent.kiev.ua/wp-content/uploads/GoPro-12-black-3.jpg",
                    Description = "Action camera with 5.3K video and HyperSmooth 6.0 stabilization"
                },
                new Equipment
                {
                    Id = 11,
                    Name = "Yamaha MG16XU Mixer",
                    CategoryId = 2,
                    PricePerHour = 30,
                    Quantity = 2,
                    IsAvailable = true,
                    ImageUrl = "https://www.hytekelectronics.co.uk/wp-content/uploads/2017/04/YAM-MG16XU.jpg",
                    Description = "16-channel professional audio mixer with USB interface and effects"
                },
                new Equipment
                {
                    Id = 12,
                    Name = "BenQ 4K Monitor 32 inch",
                    CategoryId = 8,
                    PricePerHour = 22,
                    Quantity = 4,
                    IsAvailable = true,
                    ImageUrl = "https://m.media-amazon.com/images/I/51bwiYTxx2L.jpg",
                    Description = "Ultra HD 4K monitor perfect for video editing and color grading"
                },
                new Equipment
                {
                    Id = 13,
                    Name = "Rode VideoMic Pro+",
                    CategoryId = 5,
                    PricePerHour = 12,
                    Quantity = 6,
                    IsAvailable = true,
                    ImageUrl = "https://prodj.ua/image/cache/catalog/img1b/2020/01/20200105081539-920x920.webp",
                    Description = "Professional on-camera shotgun microphone with advanced features"
                },
                new Equipment
                {
                    Id = 14,
                    Name = "Canon RF 24-70mm f/2.8 Lens",
                    CategoryId = 6,
                    PricePerHour = 35,
                    Quantity = 2,
                    IsAvailable = true,
                    ImageUrl = "https://fotosale.ua/images/products/54/products.54671.1.b.jpg",
                    Description = "Professional zoom lens with constant f/2.8 aperture"
                },
                new Equipment
                {
                    Id = 15,
                    Name = "Zoom H6 Recorder",
                    CategoryId = 2,
                    PricePerHour = 28,
                    Quantity = 3,
                    IsAvailable = true,
                    ImageUrl = "https://prodj.ua/image/cache/catalog/img3b/2020/11/20201103133205-920x920.webp",
                    Description = "6-track portable audio recorder with interchangeable capsules"
                },
                new Equipment
                {
                    Id = 16,
                    Name = "RODENSTOCK HR Digital Super MC Circular-Pol filter ",
                    CategoryId = 6,
                    PricePerHour = 45,
                    Quantity = 2,
                    IsAvailable = true,
                    ImageUrl = "https://fotosale.ua/images/products/19/products.19876.1.b.jpg",
                    Description = "The polarizing light filter increases the visual sharpness and purity of color in the photograph"
                },
                new Equipment
                {
                    Id = 17,
                    Name = "Headphones Sennheiser RS 195",
                    CategoryId = 2,
                    PricePerHour = 10,
                    Quantity = 8,
                    IsAvailable = true,
                    ImageUrl = "https://fotosale.ua/images/products/66/products.66114.1.b.jpg",
                    Description = "Professional studio monitor headphones with exceptional sound quality"
                },
                new Equipment
                {
                    Id = 18,
                    Name = "Manfrotto Tripod",
                    CategoryId = 6,
                    PricePerHour = 15,
                    Quantity = 5,
                    IsAvailable = true,
                    ImageUrl = "https://fotosale.ua/images/products/36/products.36866.1.b.jpg",
                    Description = "Heavy-duty carbon fiber tripod with fluid head for smooth camera movements"
                },
                new Equipment
                {
                    Id = 19,
                    Name = "Elgato Stream Deck Studio",
                    CategoryId = 3,
                    PricePerHour = 18,
                    Quantity = 4,
                    IsAvailable = true,
                    ImageUrl = "https://res.cloudinary.com/elgato-pwa/image/upload/q_auto,f_auto/v1725280007/Products/10GBO9901%20%28Stream%20Deck%20Studio%29/ATF/Stream-Deck-Studio-ATF-04.jpg",
                    Description = "32-key customizable control deck for streaming and content creation"
                },
                new Equipment
                {
                    Id = 20,
                    Name = "Aputure amaran Ace 25c",
                    CategoryId = 1,
                    PricePerHour = 20,
                    Quantity = 3,
                    IsAvailable = true,
                    ImageUrl = "https://fotosale.ua/images/products/67/products.67303.1.b.jpg",
                    Description = "Compact RGB constant light nameplate panel."
                }
            );
        }
    }
}
