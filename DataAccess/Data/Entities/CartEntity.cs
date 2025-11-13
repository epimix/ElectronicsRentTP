using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class CartEntity : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; } // за годину
    }
}
