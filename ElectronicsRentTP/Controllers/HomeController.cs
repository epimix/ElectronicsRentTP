using BusinessLogic.Interfaces;
using DataAccess.Data;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using BusinessLogic.Dtos;

namespace ElectronicsRentTP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEquipmentService eq;

        public HomeController(IEquipmentService eq)
        {
            this.eq = eq;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;
            var equipment = await eq.GetAll(null, null, null, null, null, null, null, page);
            var totalCount = await eq.GetTotalCount(null, null, null, null, null, null);

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

            return View(result);
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
