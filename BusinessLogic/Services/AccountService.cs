using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using ElectronicsRentTP.Helpers;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserServices userService;
        private readonly UserManager<User> userManager;
        private readonly IJwtService jwtService;

        public AccountService(
            IUserServices userService,
            UserManager<User> userManager,
            IJwtService jwtService)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.jwtService = jwtService;
        }

        public async Task<User> Register(RegisterViewModel model)
        {
            string? finalImagePath = null;

            if (model.profileImageFile != null && model.profileImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.profileImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.profileImageFile.CopyToAsync(stream);
                }

                finalImagePath = "/uploads/" + fileName;
            }
            else if (!string.IsNullOrWhiteSpace(model.profileImageUrl))
            {
                finalImagePath = model.profileImageUrl;
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Birthdate = model.Birthdate,
                profilePicture = finalImagePath
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            if (user.Email.Contains("admin"))
                await userManager.AddToRoleAsync(user, Roles.ADMIN);
            else
                await userManager.AddToRoleAsync(user, Roles.USER);

            return user;
        }

        public async Task<(User user, string jwtToken, string refreshToken)> Login(LoginViewModel model, string ipAddress)
        {
            var user = await userManager.FindByNameAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                throw new Exception("wrong login or password");

            var refreshToken = jwtService.GenerateRefreshToken(ipAddress ?? "unknown");
            user.RefreshTokens.Add(refreshToken);

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new Exception(updateResult.Errors.First().Description);


            var claims = jwtService.GetClaims(user);
            var jwtToken = jwtService.GenerateToken(claims);

            return (user, jwtToken, refreshToken.Token);
        }

        public async Task<UserProfileViewModel> GetProfile(User user)
        {
            var roles = await userManager.GetRolesAsync(user);

            return new UserProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Birthdate = user.Birthdate,
                Roles = roles.ToList(),
                ProfilePicture = user.profilePicture,
                IsAuthenticated = true
            };
        }
        public async Task UpdateProfile(User user, UserProfileViewModel model, IFormFile? profileImageFile, string? profileImageUrl)
        {
            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName;

            if (model.Birthdate.HasValue)
                user.Birthdate = model.Birthdate;

            if (profileImageFile != null && profileImageFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid() + Path.GetExtension(profileImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await profileImageFile.CopyToAsync(stream);

                user.profilePicture = "/uploads/" + fileName;
            }
            else if (!string.IsNullOrWhiteSpace(profileImageUrl))
            {
                user.profilePicture = profileImageUrl;
            }

            await userManager.UpdateAsync(user);
        }
    }
}
