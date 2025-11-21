using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dtos
{
    public class EquipmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal AverageRating { get; set; }
        public bool IsAvailable { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }

        public string? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerAvatarUrl { get; set; }

    }
}
