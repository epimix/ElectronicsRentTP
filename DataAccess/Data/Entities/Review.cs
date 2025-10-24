using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class Review // Відгуки про техніку після завершення оренди
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int Rating { get; set; } // 1–5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
