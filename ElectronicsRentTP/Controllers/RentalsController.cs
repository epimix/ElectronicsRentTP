using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Enum;
using ElectronicsRentTP.Models;
using ElectronicsRentTP.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsRentTP.Controllers
{
    public class RentalsController : Controller
    {
        private readonly EquipmentRentalDbContext _ctx;
        private readonly UserManager<User> _userManager;


        public RentalsController(EquipmentRentalDbContext ctx, UserManager<User> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Create(int equipmentId)
        {
            var equipment = await _ctx.Equipments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == equipmentId);

            if (equipment == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("can't find equipment", ToastType.danger));
                return RedirectToAction("Index", "Equipment");
            }

            var vm = new CreateRentalViewModel
            {
                EquipmentId = equipment.Id,
                EquipmentName = equipment.Name,
                PricePerHour = equipment.PricePerHour,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1)
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRentalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                
                var eq = await _ctx.Equipments.AsNoTracking().FirstOrDefaultAsync(e => e.Id == model.EquipmentId);
                if (eq != null)
                {
                    model.EquipmentName = eq.Name;
                    model.PricePerHour = eq.PricePerHour;
                }
                return View(model);
            }

            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError(string.Empty, "end date must be later than start date.");
                var eq = await _ctx.Equipments.AsNoTracking().FirstOrDefaultAsync(e => e.Id == model.EquipmentId);
                if (eq != null)
                {
                    model.EquipmentName = eq.Name;
                    model.PricePerHour = eq.PricePerHour;
                }
                return View(model);
            }

            var equipment = await _ctx.Equipments.FirstOrDefaultAsync(e => e.Id == model.EquipmentId);
            if (equipment == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("can't find an equipment", ToastType.danger));
                return RedirectToAction("Index", "Equipment");
            }
            if (equipment.Status == EquipmentStatus.Rented || equipment.Status == EquipmentStatus.Reserved)
            {
                ModelState.AddModelError(string.Empty, "This equipment is currently not available.");
                model.EquipmentName = equipment.Name;
                model.PricePerHour = equipment.PricePerHour;
                return View(model);
            }
            var overlappingRentalsCount = await _ctx.Rentals
                .Where(r => r.EquipmentId == equipment.Id
                            && r.Status != RentalStatus.Cancelled
                            && r.Status != RentalStatus.Rejected

                            && r.EndDate >= model.StartDate
                            && r.StartDate <= model.EndDate)
                .CountAsync();

            if (overlappingRentalsCount >= equipment.Quantity)
            {
                ModelState.AddModelError(string.Empty, "this dates can't be booked.");
                model.EquipmentName = equipment.Name;
                model.PricePerHour = equipment.PricePerHour;
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("you must login to book something.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            var totalHours = (decimal)(model.EndDate - model.StartDate).TotalHours;
            if (totalHours <= 0) totalHours = 24m;

            var totalPrice = Math.Round(totalHours * equipment.PricePerHour, 2);

            var rental = new Rental
            {
                EquipmentId = equipment.Id,
                UserId = userId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = RentalStatus.Pending,
                TotalPrice = totalPrice
            };

            _ctx.Rentals.Add(rental);
            await _ctx.SaveChangesAsync();

            TempData.Set(WebConstants.ToastMessage, new ToastModel("Booking created, wait for an confirmation.", ToastType.success));
            return RedirectToAction("Details", new { id = rental.Id });
        }

        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("you must login into system.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            var rentals = await _ctx.Rentals
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Include(r => r.Equipment)
                .OrderByDescending(r => r.StartDate)
                .Select(r => new RentalListItemViewModel
                {
                    Id = r.Id,
                    EquipmentId = r.EquipmentId,
                    EquipmentName = r.Equipment.Name,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    Status = r.Status,
                    TotalPrice = r.TotalPrice
                })
                .ToListAsync();

            return View(rentals);
        }


        public async Task<IActionResult> Details(int id)
        {
            var rental = await _ctx.Rentals
                .AsNoTracking()
                .Include(r => r.Equipment)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("No booking found", ToastType.danger));
                return RedirectToAction("Index", "Home");
            }

            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null || (rental.UserId != currentUserId /* && !User.IsInRole("Admin") */))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Don't have access to this booking", ToastType.danger));
                return RedirectToAction("Index", "Home");
            }

            var vm = new RentalDetailsViewModel
            {
                Id = rental.Id,
                EquipmentId = rental.EquipmentId,
                EquipmentName = rental.Equipment?.Name ?? "—",
                StartDate = rental.StartDate,
                EndDate = rental.EndDate,
                Status = rental.Status,
                TotalPrice = rental.TotalPrice,
                UserEmail = rental.User?.Email,
                EquipmentImageUrl = rental.Equipment?.ImageUrl
            };

            return View(vm);
        }
    }
}
