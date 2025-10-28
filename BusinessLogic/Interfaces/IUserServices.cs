using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IUserServices
    {

            Task Register(User user, string password);
            Task<User?> Login(string login, string password, string? ipAddress);
            Task Logout();
            Task<List<User>> GetAll();
            Task<User?> GetById(string id);
            Task Delete(string id);
            Task Update(User user);
            Task ResetPassword(string email, string newPassword);
        
    }
}
