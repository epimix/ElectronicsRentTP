using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Data.Entities;
using BusinessLogic.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;
using BusinessLogic.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ElectronicsRentTP.Helpers;

namespace ElectronicsRentTP.Controllers
{
    [Authorize] // але я залогінений в тому і прикол
    public class EquipmentController : Controller
    {
        private readonly EquipmentRentalDbContext ctx;
        private readonly IEquipmentService eq;
        private readonly IMapper mapper;
        private readonly IReviewService reviewService;
        private readonly UserManager<User> userManager;
        private readonly IRentalService rentalService;

        public EquipmentController(
            EquipmentRentalDbContext ctx,
            IEquipmentService eq,
            IMapper mapper,
            IReviewService reviewService,
            UserManager<User> userManager,
            IRentalService rentalService)
        {
            this.ctx = ctx;
            this.eq = eq;
            this.mapper = mapper;
            this.reviewService = reviewService;
            this.userManager = userManager;
            this.rentalService = rentalService;
        }
        //[Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> Index(
            int page = 1,
            int? categoryId = null,
            string? searchName = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isAvailable = null,
            bool? sortPriceAsc = null)
        {
            const int pageSize = 10;
            var models = await eq.GetAll(categoryId, searchName, null, minPrice, maxPrice, sortPriceAsc, isAvailable, page);
            var totalCount = await eq.GetTotalCount(categoryId, searchName, null, minPrice, maxPrice, isAvailable);

            var pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };

            var result = new PagedResult<EquipmentDTO>
            {
                Items = models.ToList(),
                Pagination = pagination
            };

            // Set filter values to ViewBag for form
            SetCategoriesToViewBag();
            ViewBag.CategoryId = categoryId;
            ViewBag.SearchName = searchName;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.IsAvailable = isAvailable;
            ViewBag.SortPriceAsc = sortPriceAsc;

            await rentalService.AutoCompleteRentalsAsync();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var equipment = await eq.GetById(id);
            if (equipment == null) return NotFound();

            var equipmentDto = mapper.Map<EquipmentDTO>(equipment);

            if (!string.IsNullOrEmpty(equipment.OwnerId))
            {
                var owner = await ctx.Users
                    .FirstOrDefaultAsync(u => u.Id == equipment.OwnerId);

                if (owner != null)
                {
                    equipmentDto.OwnerId = owner.Id;
                    equipmentDto.OwnerName =
                        !string.IsNullOrWhiteSpace(owner.FullName)
                            ? owner.FullName
                            : (!string.IsNullOrWhiteSpace(owner.Login)
                                ? owner.Login
                                : owner.Email);

                    equipmentDto.OwnerAvatarUrl = owner.profilePicture;
                }
            }
            var reviews = await reviewService.GetReviewsByEquipment(id);

            ViewBag.Reviews = reviews.OrderByDescending(r => r.CreatedAt).ToList();
            ViewBag.EquipmentId = id;
            ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;

            return View(equipmentDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int equipmentId, int rating, string comment)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (rating < 1 || rating > 5)
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Rating must be between 1 and 5", ToastType.danger));
                return RedirectToAction("Details", new { id = equipmentId });
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData.Set(WebConstants.ToastMessage, new ToastModel("Comment cannot be empty", ToastType.danger));
                return RedirectToAction("Details", new { id = equipmentId });
            }

            await reviewService.PostReview(user.Id, equipmentId, comment, rating);
            TempData.Set(WebConstants.ToastMessage, new ToastModel("Review added successfully!"));

            return RedirectToAction("Details", new { id = equipmentId });
        }

        [HttpGet]
        //[Authorize(Roles = Roles.ADMIN)]
        public IActionResult Create()
        {
            SetCategoriesToViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> Create(EquipmentDTO equipment)
        {
            if (!ModelState.IsValid)
            {
                SetCategoriesToViewBag();
                return View(equipment);
            }
            var userId = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userId);
            Equipment equ = mapper.Map<Equipment>(equipment);

            equ.OwnerId = userId;
            await eq.AddEquipment(equ, userId);
            TempData.Set(WebConstants.ToastMessage, new ToastModel("eq created successfully"));

            return RedirectToAction("Index");
            // але коли через адміна то товар норм додається
        }
                // але товар добавляється
        [HttpGet]
        //[Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> Edit(int id)
        {
            var equipment = await eq.GetById(id);
            if (equipment == null) return NotFound();

            SetCategoriesToViewBag();
            return View(mapper.Map<EquipmentDTO>(equipment));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> Edit(EquipmentDTO equipment)
        {
            if (!ModelState.IsValid)
            {
                SetCategoriesToViewBag();
                return View(equipment);
            }

            await eq.UpdateEquipment(mapper.Map<Equipment>(equipment));
            TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment updated successfully!"));

            return RedirectToAction("Index");
        }

        //[Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> Delete(int id)
        {
            await eq.DeleteEquipment(id);
            TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment deleted successfully!"));

            return RedirectToAction("Index");
        }


        private void SetCategoriesToViewBag()
        {
            var categories = new SelectList(ctx.EquipmentCategories.ToList(), "Id", "Name");
            ViewBag.Categories = categories;
        }
    }
}
