using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;
using System.Threading.Tasks;

namespace ElectronicsRentTP.Controllers
{

    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<User> _userManager;

        public CartController(ICartService cartService, UserManager<User> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to view your cart.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _cartService.GetCartItems(userId);
            return View(cartItems);
        }

        // POST: /Cart/Add
        [HttpPost]
        public async Task<IActionResult> Add(int equipmentId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to add items to the cart.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _cartService.AddToCart(userId, equipmentId, quantity);
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Item added to cart.", ToastType.success));
            }
            catch (System.Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
            }

            return RedirectToAction("Index", "Equipment");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public async Task<IActionResult> Remove(int equipmentId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to remove items.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _cartService.RemoveFromCart(userId, equipmentId);
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Item removed from cart.", ToastType.success));
            }
            catch (System.Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to clear the cart.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var cartItems = await _cartService.GetCartItems(userId);
                foreach (var item in cartItems)
                {
                    await _cartService.RemoveFromCart(userId, item.EquipmentId);
                }
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Cart cleared.", ToastType.success));
            }
            catch (System.Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
