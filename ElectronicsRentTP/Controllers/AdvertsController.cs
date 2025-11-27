using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Data.Entities;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;
using ElectronicsRentTP.Helpers;
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
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ICategoryService categoryService;

        public AdvertsController(
            IAdvertsService advertsService,
            UserManager<User> userManager,
            IMapper mapper,
            ICategoryService categoryService)
        {
            _advertsService = advertsService;
            _userManager = userManager;
            _mapper = mapper;
            this.categoryService = categoryService;
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
        public async Task<IActionResult> Create(EquipmentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await categoryService.GetAll());
                return View(dto);
            }

            var userId = _userManager.GetUserId(User);
            await _advertsService.CreateAdvert(dto, userId);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var eq = await _advertsService.GetById(id);

            if (eq == null || !_advertsService.CanEdit(eq, userId, isAdmin))
                return RedirectToAction(nameof(Index));

            SetCategoriesToViewBag();

            return View(_mapper.Map<EquipmentDTO>(eq));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EquipmentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                SetCategoriesToViewBag();
                return View(dto);
            }

            await _advertsService.UpdateAdvert(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var eq = await _advertsService.GetById(id);

            if (eq != null && isAdmin)
                await _advertsService.DeleteAdvert(id);

            return RedirectToAction(nameof(Index));
        }

        private async void SetCategoriesToViewBag()
        {
            var categories = new SelectList( await categoryService.GetAll());
            ViewBag.Categories = categories;
        }
    }
}
