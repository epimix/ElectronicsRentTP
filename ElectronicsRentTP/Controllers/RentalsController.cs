using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Data.Entities;
using ElectronicsRentTP.Models;
using ElectronicsRentTP.Extensions;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BusinessLogic.Services;

namespace ElectronicsRentTP.Controllers
{
    public class RentalsController : Controller
    {
        private readonly EquipmentRentalDbContext _ctx;
        private readonly UserManager<User> _userManager;
        private readonly IRentalService _rentalService;
        

        public RentalsController(EquipmentRentalDbContext ctx, UserManager<User> userManager, IRentalService rentalService)
        {
            _ctx = ctx;
            _userManager = userManager;
            _rentalService = rentalService;
        }
        [Authorize]
        public async Task<IActionResult> Create(int equipmentId)
        {
            var vm = await _rentalService.InitializeRentalAsync(equipmentId);
            if (vm == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Can't find equipment.", ToastType.danger));
                return RedirectToAction("Index", "Home");
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRentalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEquipmentFields(model);
                return View(model);
            }

            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be later than start date.");
                await PopulateEquipmentFields(model);
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to book something.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            var (success, errorMessage, rental) = await _rentalService.CreateRentalAsync(model, userId);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                await PopulateEquipmentFields(model);
                return View(model);
            }

            TempData.Set(WebConstants.ToastMessage, new ToastModel("Booking created. Await confirmation.", ToastType.success));
            return RedirectToAction("Details", new { id = rental!.Id });
        }

        private async Task PopulateEquipmentFields(CreateRentalViewModel model)
        {
            var eq = await _rentalService.GetEquipmentDetailsAsync(model.EquipmentId);
            model.EquipmentName = eq.Name;
            model.PricePerHour = eq.PricePerHour;
        }

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            await _rentalService.AutoCompleteRentalsAsync();

            var userId = _userManager.GetUserId(User);
            var rentals = await _rentalService.GetUserRentalsAsync(userId!, 1);
            return View(rentals);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var rentalDetails = await _rentalService.GetRentalDetailsAsync(id, userId!);

            if (rentalDetails == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Access denied or booking not found.", ToastType.danger));
                return RedirectToAction("Index", "Home");
            }

            return View(rentalDetails);
        }

        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var Task = await _rentalService.DeleteRentalAsync(id, userId!);
            var rentals = await _rentalService.GetUserRentalsAsync(userId!, 1);
            return RedirectToAction("MyBookings");
        }
    }
}
