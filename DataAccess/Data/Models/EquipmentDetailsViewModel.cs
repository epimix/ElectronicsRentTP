using DataAccess.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Models
{
    public class EquipmentDetailsViewModel
    {
        public Equipment Equipment { get; set; }

        public IEnumerable<Review> Comments { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public string? NewComment { get; set; }
    }
}
