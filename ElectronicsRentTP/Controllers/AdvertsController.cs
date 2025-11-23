using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Data.Entities;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using BusinessLogic.Dtos;
using AutoMapper;

namespace ElectronicsRentTP.Controllers
{
    public class AdvertsController : Controller
    {
        private readonly IAdvertsService _advertsService;
        private readonly IEquipmentService _equipmentService;
        private readonly UserManager<User> _userManager;
        private readonly EquipmentRentalDbContext _ctx;
        private readonly IMapper _mapper;

        public AdvertsController(
            IAdvertsService advertsService,
            IEquipmentService equipmentService,
            UserManager<User> userManager,
            EquipmentRentalDbContext ctx,
            IMapper mapper)
        {
            _advertsService = advertsService;
            _equipmentService = equipmentService;
            _userManager = userManager;
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You must login to view your adverts.", ToastType.info));
                return RedirectToAction("Login", "Account");
            }

            var adverts = await _advertsService.GetCartItems(userId);
            return View(adverts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetCategoriesToViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipmentDTO equipment)
        {
            if (!ModelState.IsValid)
            {
                SetCategoriesToViewBag();
                return View(equipment);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                Equipment equ = _mapper.Map<Equipment>(equipment);
                equ.OwnerId = userId;
                await _equipmentService.AddEquipment(equ, userId);
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment created successfully!", ToastType.success));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
                SetCategoriesToViewBag();
                return View(equipment);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var equipment = await _equipmentService.GetById(id);
            if (equipment == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment not found.", ToastType.danger));
                return RedirectToAction(nameof(Index));
            }

            // Перевіряємо доступ: користувач може редагувати тільки свої товари
            // Товари з БД (OwnerId == null або адмін) можуть редагувати тільки адміни
            var isAdmin = User.IsInRole("Admin");
            var isFromDatabase = string.IsNullOrEmpty(equipment.OwnerId) ||
                                equipment.OwnerId == "551b73c1-3601-490c-90bf-5af17a4408d5";

            if (!isAdmin && (isFromDatabase || equipment.OwnerId != userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You don't have permission to edit this equipment.", ToastType.danger));
                return RedirectToAction(nameof(Index));
            }

            SetCategoriesToViewBag();
            return View(_mapper.Map<EquipmentDTO>(equipment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EquipmentDTO equipment)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var existingEquipment = await _equipmentService.GetById(equipment.Id);
            if (existingEquipment == null)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment not found.", ToastType.danger));
                return RedirectToAction(nameof(Index));
            }

            // Перевіряємо доступ: користувач може редагувати тільки свої товари
            // Товари з БД (OwnerId == null або адмін) можуть редагувати тільки адміни
            var isAdmin = User.IsInRole("Admin");
            var isFromDatabase = string.IsNullOrEmpty(existingEquipment.OwnerId) ||
                                existingEquipment.OwnerId == "551b73c1-3601-490c-90bf-5af17a4408d5";

            if (!isAdmin && (isFromDatabase || existingEquipment.OwnerId != userId))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("You don't have permission to edit this equipment.", ToastType.danger));
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                SetCategoriesToViewBag();
                return View(equipment);
            }

            try
            {
                // Оновлюємо існуючий об'єкт замість створення нового
                existingEquipment.Name = equipment.Name;
                existingEquipment.Description = equipment.Description;
                existingEquipment.CategoryId = equipment.CategoryId;
                existingEquipment.PricePerHour = equipment.PricePerHour;
                existingEquipment.Quantity = equipment.Quantity;
                existingEquipment.ImageUrl = equipment.ImageUrl;
                // OwnerId залишаємо без змін

                await _equipmentService.UpdateEquipment(existingEquipment);
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment updated successfully!", ToastType.success));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
                SetCategoriesToViewBag();
                return View(equipment);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var equipment = await _equipmentService.GetById(id);
            if (equipment == null || equipment.OwnerId != userId)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment not found or access denied.", ToastType.danger));
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _equipmentService.DeleteEquipment(id);
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment deleted successfully!", ToastType.success));
            }
            catch (Exception ex)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel(ex.Message, ToastType.danger));
            }

            return RedirectToAction(nameof(Index));
        }

        private void SetCategoriesToViewBag()
        {
            var categories = new SelectList(_ctx.EquipmentCategories.ToList(), "Id", "Name");
            ViewBag.Categories = categories;
        }
    }
}
