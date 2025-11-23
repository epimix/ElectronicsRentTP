using BusinessLogic.Dtos;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Models;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsRentTP.Controllers
{
    public class UsersController : Controller
    {
        private readonly EquipmentRentalDbContext _ctx;

        public UsersController(EquipmentRentalDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _ctx.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            // Всі оголошення цього юзера
            var adverts = await _ctx.Equipments
                .Where(e => e.OwnerId == id)
                .Select(e => new Equipment
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    PricePerHour = e.PricePerHour,
                    IsAvailable = e.IsAvailable,
                    ImageUrl = e.ImageUrl
                })
                .ToListAsync();

            var displayName = !string.IsNullOrWhiteSpace(user.FullName)
                ? user.FullName
                : (!string.IsNullOrWhiteSpace(user.Login)
                    ? user.Login
                    : user.Email);

            var model = new OwnerProfileViewModel
            {
                UserId = user.Id,
                DisplayName = displayName ?? "Unknown user",
                ProfilePicture = user.profilePicture,
                LastOnline = user.LastOnline,      // додамо поле далі
                IsOnline = user.LastOnline.HasValue &&
                           DateTime.UtcNow - user.LastOnline.Value < TimeSpan.FromMinutes(5),
                Adverts = adverts
            };

            return View(model);
        }
    }
}
