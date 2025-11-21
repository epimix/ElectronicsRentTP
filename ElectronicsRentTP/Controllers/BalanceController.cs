using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using DataAccess.Data.Entities;

namespace ElectronicsRentTP.Controllers
{
   // [Authorize]
    public class BalanceController : Controller
    {
        private readonly IBalanceService _balanceService;
        private readonly UserManager<User> _userManager;

        public BalanceController(IBalanceService balanceService, UserManager<User> userManager)
        {
            _balanceService = balanceService;
            _userManager = userManager;
        }

        // GET: /Balance
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            try
            {
                var balance = await _balanceService.GetUserBalanceAsync(userId);
                ViewBag.Balance = balance;
                return View();
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }

        // POST: /Balance/Replenish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Replenish(decimal amount, string cardNumber, string cardHolder, string expiryDate, string cvv)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Валідація суми
            if (amount <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return RedirectToAction(nameof(Index));
            }

            // Валідація даних карти
            if (string.IsNullOrWhiteSpace(cardNumber) ||
                string.IsNullOrWhiteSpace(cardHolder) ||
                string.IsNullOrWhiteSpace(expiryDate) ||
                string.IsNullOrWhiteSpace(cvv))
            {
                TempData["Error"] = "Please fill in all card details.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Імітація обробки платежу
                await Task.Delay(500);

                // Поповнення балансу
                await _balanceService.ReplenishmentBalanceAsync(userId, amount);

                TempData["Success"] = $"Balance successfully replenished by ${amount:F2}! 💰";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"Error replenishing balance: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
