using DataAccess.Data.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAdminService
    {
        Task<List<Complaint>> GetComplaints();
        Task ResolveComplaint(int id);
        Task BanUser(string id);
        Task DeleteUser(string id);
        Task DeleteAdvert(int id);
        Task AddComment(int id, string comment);
        Task Refound(int id);

    }
}
