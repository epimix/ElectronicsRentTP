using System;
using System.Collections.Generic;

namespace ElectronicsRentTP.Models
{
    public class UserProfileViewModel
    {
        public bool IsAuthenticated { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? Birthdate { get; set; }

        public string? ProfilePicture { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}


