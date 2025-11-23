using System;
using System.Collections.Generic;
using DataAccess.Data.Entities;

namespace DataAccess.Data.Models
{
    public class OwnerProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;

        // Те, що ти показуєш як нікнейм (FullName / Login / Email)
        public string DisplayName { get; set; } = string.Empty;

        public string? ProfilePicture { get; set; }

        // Онлайн-статус
        public bool IsOnline { get; set; }
        public DateTime? LastOnline { get; set; }

        // Всі його оголошення
        public IList<Equipment> Adverts { get; set; } = new List<Equipment>();
    }
}
