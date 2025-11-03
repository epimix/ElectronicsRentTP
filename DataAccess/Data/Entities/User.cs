using Microsoft.AspNetCore.Identity;
using System;
namespace DataAccess.Data.Entities
{
    public class User : IdentityUser, BaseEntity
    {
        public string Login { get; set; } = string.Empty;
        public string? FullName { get; set; } = "";
        public DateTime? Birthdate { get; set; }
        public decimal Balance { get; set; } = 0;

        public List<Equipment> FavoriteEquipment { get; set; } = new List<Equipment>();

        public List<Rental> Rentals { get; set; } = new List<Rental>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
