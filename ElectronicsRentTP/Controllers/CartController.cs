using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;
using System.Threading.Tasks;
using System.Linq;

namespace ElectronicsRentTP.Controllers
{

    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IRentalService _rentalService;
        private readonly UserManager<User> _userManager;

        public CartController(ICartService cartService, IRentalService rentalService, UserManager<User> userManager)
        {
            _cartService = cartService;
            _rentalService = rentalService;
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
        [ValidateAntiForgeryToken]
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

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int equipmentId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to update cart.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _cartService.UpdateQuantity(userId, equipmentId, quantity);
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Cart updated.", ToastType.success));
            }
            catch (System.Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(DateTime? startDate = null, DateTime? endDate = null)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to checkout.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _cartService.GetCartItems(userId);
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Your cart is empty.", ToastType.warning));
                return RedirectToAction(nameof(Index));
            }

            // Якщо дати не вказані, використовуємо за замовчуванням
            var start = startDate ?? DateTime.UtcNow.Date;
            var end = endDate ?? DateTime.UtcNow.Date.AddDays(1);

            try
            {
                // Створюємо оренди для всіх товарів в кошику
                foreach (var item in cartItems)
                {
                    var model = new CreateRentalViewModel
                    {
                        EquipmentId = item.EquipmentId,
                        StartDate = start,
                        EndDate = end,
                        PaymentType = DataAccess.Data.Enum.PaymentType.creditCard,
                        Description = $"Rental from cart - {item.Quantity} items"
                    };

                    var (success, errorMessage, rental) = await _rentalService.CreateRentalAsync(model, userId);
                    if (!success)
                    {
                        TempData.Set(WebConstants.ToastMessage, new ToastModel($"Error creating rental for {item.Equipment?.Name}: {errorMessage}", ToastType.danger));
                        return RedirectToAction(nameof(Index));
                    }

                    // Видаляємо товар з кошика після успішного створення оренди
                    await _cartService.RemoveFromCart(userId, item.EquipmentId);
                }

                TempData.Set(WebConstants.ToastMessage, new ToastModel("All items checked out successfully!", ToastType.success));
                return RedirectToAction("MyBookings", "Rentals");
            }
            catch (System.Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
