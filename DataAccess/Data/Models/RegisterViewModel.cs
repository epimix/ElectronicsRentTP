using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsRentTP.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public IFormFile? profileImageFile { get; set; }
        public string? profileImageUrl { get; set; }


        [Required]
        public string FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }
    }
}
