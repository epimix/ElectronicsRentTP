using Azure;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


namespace ElectronicsRentTP.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAccountService _accountService;

        public AccountController(IAccountService _accountService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this._accountService = _accountService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _accountService.Register(model);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                var (user, jwtToken, refreshToken) =
                    await _accountService.Login(model, ip);

                var isHttps = Request.IsHttps;

                Response.Cookies.Append("sessionToken", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = isHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(24)
                });

                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("sessionToken"))
                Response.Cookies.Delete("sessionToken");

            HttpContext.Session?.Clear();


            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return View(new UserProfileViewModel { IsAuthenticated = false });

            var user = await _userManager.GetUserAsync(User);
            var model = await _accountService.GetProfile(user);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model, IFormFile? profileImageFile, string? profileImageUrl)
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Login");

            var user = await _userManager.GetUserAsync(User);

            try
            {
                await _accountService.UpdateProfile(user, model, profileImageFile, profileImageUrl);
                TempData["Success"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating profile: {ex.Message}";
            }

            return RedirectToAction(nameof(Profile));
        }
    }
}
