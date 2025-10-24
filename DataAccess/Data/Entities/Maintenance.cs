using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class Maintenance : BaseEntity // Технічне обслуговування техніки.
    {
        public int Id { get; set; }

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
    }

}
