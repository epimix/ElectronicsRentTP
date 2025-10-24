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


    }
}
