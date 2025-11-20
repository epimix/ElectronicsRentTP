using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using DataAccess.Data.Entities;

namespace ElectronicsRentTP.Controllers
{
    [Authorize]
    public class OwnerRentalsController : Controller
    {
        private readonly IRentalService _rentalService;
        private readonly UserManager<User> _userManager;

        public OwnerRentalsController(IRentalService rentalService, UserManager<User> userManager)
        {
            _rentalService = rentalService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var rentals = await _rentalService.GetNotConfirmRental(userId);
            return View(rentals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var ownerRentals = await _rentalService.GetNotConfirmRental(userId);
            var rental = ownerRentals.FirstOrDefault(r => r.Id == id);

            if (rental == null)
            {
                TempData["Error"] = "Rental not found or access denied.";
                return RedirectToAction(nameof(Index));
            }

            await _rentalService.ConfirmRental(id);
            TempData["Success"] = "Rental approved successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var ownerRentals = await _rentalService.GetNotConfirmRental(userId);
            var rental = ownerRentals.FirstOrDefault(r => r.Id == id);

            if (rental == null)
            {
                TempData["Error"] = "Rental not found or access denied.";
                return RedirectToAction(nameof(Index));
            }

            await _rentalService.RejectRental(id);
            TempData["Success"] = "Rental rejected!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var ownerRentals = await _rentalService.GetNotConfirmRental(userId);
            var rental = ownerRentals.FirstOrDefault(r => r.Id == id);

            if (rental == null)
            {
                TempData["Error"] = "Rental not found or access denied.";
                return RedirectToAction(nameof(Index));
            }

            await _rentalService.CancelRental(id);
            TempData["Success"] = "Rental cancelled!";
            return RedirectToAction(nameof(Index));
        }
    }
}
