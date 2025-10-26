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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId);
            modelBuilder.Entity<Equipment>()
                .Property(e => e.PricePerHour)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Maintenance>()
                .Property(m => m.Cost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Rental>()
                .Property(r => r.TotalPrice)
                .HasColumnType("decimal(18,2)");

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
                }
            );
        }
    }
}
