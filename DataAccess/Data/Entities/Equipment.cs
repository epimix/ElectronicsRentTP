using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class Equipment : BaseEntity // Одиниця спецтехніки.
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public EquipmentCategory Category { get; set; }

        public decimal PricePerHour { get; set; }
        public bool IsAvailable { get; set; }
        public int Quantity { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<Rental> Rentals { get; set; }
    }
}
