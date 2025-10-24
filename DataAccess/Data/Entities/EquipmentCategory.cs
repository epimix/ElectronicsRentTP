using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class EquipmentCategory : BaseEntity // Категорія техніки
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Equipment> Equipments { get; set; }
    }
}
