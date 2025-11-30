using BusinessLogic.Interfaces;
using DataAccess.Data;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using BusinessLogic.Dtos;
using Microsoft.Extensions.Logging;

namespace ElectronicsRentTP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEquipmentService eq;
        private readonly ICategoryService categoryService;
        private readonly EquipmentRentalDbContext ctx;
        private readonly ILogger<HomeController> logger;

        public HomeController(IEquipmentService eq, ICategoryService categoryService, EquipmentRentalDbContext ctx, ILogger<HomeController> logger)
        {
            this.eq = eq;
            this.categoryService = categoryService;
            this.ctx = ctx;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            int? categoryId = null,
            string? searchName = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isAvailable = null,
            bool? sortPriceAsc = null)
        {
            try
            {
                const int pageSize = 10;
                var equipment = await eq.GetAll(categoryId, searchName, null, minPrice, maxPrice, sortPriceAsc, isAvailable, page);
                var totalCount = await eq.GetTotalCount(categoryId, searchName, null, minPrice, maxPrice, isAvailable);

            var pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };

            var result = new PagedResult<BusinessLogic.Dtos.EquipmentDTO>
            {
                Items = equipment.ToList(),
                Pagination = pagination
            };

                // Set filter values to ViewBag for form
                ViewBag.Categories = new SelectList(ctx.EquipmentCategories.ToList(), "Id", "Name", categoryId);
                ViewBag.CategoryId = categoryId;
                ViewBag.SearchName = searchName;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.IsAvailable = isAvailable;
                ViewBag.SortPriceAsc = sortPriceAsc;

                return View(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading equipment data. Connection string issue? Error: {Error}", ex.Message);
                
                // Log connection string info
                var connectionString = ctx.Database.GetConnectionString();
                if (connectionString != null)
                {
                    var masked = connectionString.Contains("Password=") 
                        ? connectionString.Substring(0, connectionString.IndexOf("Password=")) + "Password=***" 
                        : connectionString;
                    logger.LogError("Current connection string: {ConnectionString}", masked);
                }
                else
                {
                    logger.LogError("Connection string is NULL!");
                }
                
                // Return empty result instead of crashing
                var emptyResult = new PagedResult<BusinessLogic.Dtos.EquipmentDTO>
                {
                    Items = new List<BusinessLogic.Dtos.EquipmentDTO>(),
                    Pagination = new PaginationInfo { CurrentPage = 1, PageSize = 10, TotalItems = 0 }
                };
                ViewBag.Categories = new SelectList(new List<DataAccess.Data.Entities.EquipmentCategory>(), "Id", "Name");
                return View(emptyResult);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
