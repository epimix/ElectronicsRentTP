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
        private readonly IUserServices _userService;
        private readonly IJwtService _tokenService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IJwtService tokenService, IUserServices userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userService = userService;
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

            string? finalImagePath = null;


            if (model.profileImageFile != null && model.profileImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.profileImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.profileImageFile.CopyToAsync(stream);
                }

                finalImagePath = "/uploads/" + fileName;
            }

            else if (!string.IsNullOrWhiteSpace(model.profileImageUrl))
            {
                finalImagePath = model.profileImageUrl;
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Birthdate = model.Birthdate,
                profilePicture = finalImagePath
            };

            try
            {
                await _userService.Register(user, model.Password);

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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

                var user = await _userService.Login(model.Email, model.Password,
                                                    HttpContext.Connection.RemoteIpAddress?.ToString());


                var claims = _tokenService.GetClaims(user);
                var jwtToken = _tokenService.GenerateToken(claims);

                // Встановлюємо cookie з правильними опціями
                var isHttps = Request.IsHttps;
                Response.Cookies.Append("sessionToken", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = isHttps, // Secure тільки для HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(24) // Токен на 24 години
                });

                // Також встановлюємо стандартну автентифікацію ASP.NET Identity
                await _signInManager.SignInAsync(user, isPersistent: false);


                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
            var model = new UserProfileViewModel
            {
                IsAuthenticated = User?.Identity?.IsAuthenticated ?? false
            };

            if (!model.IsAuthenticated)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.FullName = user.FullName;
                model.Email = user.Email;
                model.Birthdate = user.Birthdate;
                var roles = await _userManager.GetRolesAsync(user);
                model.Roles = roles.ToList();
                model.ProfilePicture = user.profilePicture;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model, IFormFile? profileImageFile, string? profileImageUrl)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Оновлюємо основні дані
                if (!string.IsNullOrWhiteSpace(model.FullName))
                    user.FullName = model.FullName;

                if (model.Birthdate.HasValue)
                    user.Birthdate = model.Birthdate;

                // Обробка фото профілю
                string? finalImagePath = null;

                if (profileImageFile != null && profileImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImageFile.CopyToAsync(stream);
                    }

                    finalImagePath = "/uploads/" + fileName;
                    user.profilePicture = finalImagePath;
                }
                else if (!string.IsNullOrWhiteSpace(profileImageUrl))
                {
                    user.profilePicture = profileImageUrl;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Profile updated successfully!";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating profile: {ex.Message}");
            }

            return RedirectToAction(nameof(Profile));
        }
    }
}
