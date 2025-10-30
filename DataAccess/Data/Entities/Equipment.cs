using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class Equipment : BaseEntity // Одиниця спецтехніки.
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public EquipmentCategory? Category { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal PricePerHour { get; set; }
        public int ReviewCount { get; set; } = 0;
        public int ReviewSum { get; set; } = 0;
        public decimal AverageRating { get; set; } = 0;

        public bool IsAvailable { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater")]
        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }
        public ICollection<Review> reviews { get; set; } = new List<Review>();
        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
