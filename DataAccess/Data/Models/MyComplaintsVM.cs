using DataAccess.Data.Entities;
using System.Collections.Generic;

namespace ElectronicsRentTP.Models.ViewModels
{
    public class MyComplaintsVM
    {
        public List<Complaint> SentComplaints { get; set; }
        public List<Complaint> ComplaintsAgainstMe { get; set; }
    }
}
