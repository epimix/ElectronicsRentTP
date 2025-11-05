using DataAccess.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class Rental : BaseEntity // Оренда (бронювання) техніки.
    {
        public int Id { get; set; }

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public RentalStatus Status { get; set; }

        public string? Description { get; set; }

        public PaymentType PaymentType { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
