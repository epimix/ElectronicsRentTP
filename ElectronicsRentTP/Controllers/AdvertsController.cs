using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Data.Entities;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicsRentTP.Controllers
{
    public class AdvertsController : Controller
    {
        private readonly IAdvertsService _advertsService;
        private readonly UserManager<User> _userManager;

        public AdvertsController(IAdvertsService advertsService, UserManager<User> userManager)
        {
            _advertsService = advertsService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to view your cart.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _advertsService.GetCartItems(userId);
            return View(cartItems);
        }
    }
}
