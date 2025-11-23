using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Models
{
    public class ComplaintCreateModel
    {
        public int RentalId { get; set; }
        public string TargetUserId { get; set; }
        public string Message { get; set; }
    }
}
