using DataAccess.Data.Models;
using ElectronicsRentTP.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IComplaintService
    {
        Task Create(ComplaintCreateModel model, string reporterId);
        Task<ComplaintCreateModel?> GetComplaintModel(int rentalId);
        Task<MyComplaintsVM?> GetMyComplaints(string userId);
    }
}
