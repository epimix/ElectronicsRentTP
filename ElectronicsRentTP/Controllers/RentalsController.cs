using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Enum;
using ElectronicsRentTP.Models;
using ElectronicsRentTP.Extensions;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BusinessLogic.Services;

namespace ElectronicsRentTP.Controllers
{
    public class RentalsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRentalService _rentalService;

        public RentalsController(UserManager<User> userManager, IRentalService rentalService)
        {
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
        public async Task<IActionResult> MyBookings(RentalStatus? status, int page = 1, int pageSize = 10)
        {
            await _rentalService.AutoCompleteRentalsAsync();

            var userId = _userManager.GetUserId(User);

            IEnumerable<RentalListItemViewModel> rentals;

            var targetStatus = status ?? RentalStatus.Pending;

            var detailedRentals = await _rentalService.GetRentalByStatus(userId!, targetStatus, page, pageSize);

            rentals = detailedRentals.Select(r => new RentalListItemViewModel
            {
                Id = r.Id,
                EquipmentId = r.EquipmentId,
                EquipmentName = r.EquipmentName,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Status = r.Status,
                TotalPrice = r.TotalPrice
            }).ToList();

            ViewBag.CurrentStatus = status;
            ViewBag.AllCount = await _rentalService.GetRentalCountByStatus(userId!, null, 1, 1000);
            ViewBag.PendingCount = await _rentalService.GetRentalCountByStatus(userId!, RentalStatus.Pending, 1, 1000);
            ViewBag.ApprovedCount = await _rentalService.GetRentalCountByStatus(userId!, RentalStatus.Approved, 1, 1000);
            ViewBag.CompletedCount = await _rentalService.GetRentalCountByStatus(userId!, RentalStatus.Completed, 1, 1000);
            ViewBag.RejectedCount = await _rentalService.GetRentalCountByStatus(userId!, RentalStatus.Rejected, 1, 1000);
            ViewBag.CancelledCount = await _rentalService.GetRentalCountByStatus(userId!, RentalStatus.Cancelled, 1, 1000);

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            try
            {
                await _rentalService.CancelRental(id);
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Booking cancelled successfully. Amount refunded to your balance.", ToastType.success));
            }
            catch (InvalidOperationException ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
            }
            catch (Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Error cancelling booking.", ToastType.danger));
            }

            return RedirectToAction("MyBookings");
        }
    }
}