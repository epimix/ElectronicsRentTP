using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using System.Data;
using ElectronicsRentTP.Helpers;
namespace BusinessLogic.Services
{
    public class UserServices : IUserServices
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly EquipmentRentalDbContext ctx;
        private readonly IJwtService jwtService;

        public UserServices(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            EquipmentRentalDbContext ctx,
            IJwtService jwtService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.ctx = ctx;
            this.jwtService = jwtService;
        }

        public async Task Register(User user, string password)
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.FirstOrDefault()?.Description ?? "err");

            if (user.Email.Contains("admin")) // в почті має бути admin щоб юзер став адміном
                await userManager.AddToRoleAsync(user, Roles.ADMIN);
            else
                await userManager.AddToRoleAsync(user, Roles.USER);
        }

        public async Task<User?> Login(string login, string password, string? ipAddress)
        {
            var user = await userManager.FindByNameAsync(login);
            if (user == null || !await userManager.CheckPasswordAsync(user, password))
                throw new Exception("wrong login or password");

            var refreshToken = jwtService.GenerateRefreshToken(ipAddress ?? "unknown");
            user.RefreshTokens.Add(refreshToken);

            //await userManager.AddToRoleAsync(user, Roles.ADMIN); // еслі хочете добавить адмінку на юзера з якого заходите. після використання закоментувати


            await ctx.SaveChangesAsync();
            return user;
        }

        public async Task Logout()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<List<User>> GetAll()
        {
            return await ctx.Users.ToListAsync();
        }

        public async Task<User?> GetById(string id)
        {
            return await ctx.Users
                .Include(u => u.Carts)
                .Include(u => u.Rentals)
                .Include(u => u.MyAdverts)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            await userManager.DeleteAsync(user);
        }

        public async Task Update(User user)
        {
            ctx.Users.Update(user);
            await ctx.SaveChangesAsync();
        }

        public async Task ResetPassword(string email, string newPassword)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                throw new Exception("Password reset failed");
        }
        public async Task AddToCart(CartEntity cartEntity)
        {
            var user = await ctx.Users
                .Include(u => u.Carts)
                .FirstOrDefaultAsync(u => u.Id == cartEntity.UserId);

            if (user == null)
                throw new Exception("User not found");

            user.Carts.Add(cartEntity);
            await ctx.SaveChangesAsync();
        }

        public async Task AddEquip(Equipment equipment)
        {
            var user = await ctx.Users
            .Include(u => u.MyAdverts)
            .FirstOrDefaultAsync(u => u.Id == equipment.OwnerId);
            if (!await userManager.IsInRoleAsync(user, Roles.ADMIN))
                throw new Exception("Only admins can add equipment");
            user.MyAdverts.Add(equipment);
            await ctx.SaveChangesAsync();
        }
    }
}
