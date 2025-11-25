using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class Complaint
    {
        public int Id { get; set; }

        public string ReporterId { get; set; }
        public User Reporter { get; set; }

        public string TargetUserId { get; set; }
        public User TargetUser { get; set; }

        public int RentalId { get; set; }
        public Rental Rental { get; set; }

        public string Message { get; set; }

        public string? AdminComment { get; set; }


        public DateTime Created { get; set; }

        public bool Resolved { get; set; } = false;
    }
}
