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

       
    }
}
