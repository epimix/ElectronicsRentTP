using DataAccess.Data;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ElectronicsRentTP.Controllers
{
    public class HomeController : Controller
    {
        private readonly EquipmentRentalDbContext ctx;

        public HomeController(EquipmentRentalDbContext ctx)
        {
            this.ctx = ctx;
        }

        public IActionResult Index()
        {
            var equipment = ctx.Equipments.Include(x => x.Category).ToList();
            return View(equipment);
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
