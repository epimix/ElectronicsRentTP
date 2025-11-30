using DataAccess.Data.Entities;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAccountService
    {
        Task<User> Register(RegisterViewModel model);
        Task<(User user, string jwtToken, string refreshToken)> Login(LoginViewModel model, string ipAddress);
        Task UpdateProfile(User user, UserProfileViewModel model, IFormFile? profileImageFile, string? profileImageUrl);
        Task<UserProfileViewModel> GetProfile(User user);


    }
}
