using BusinessLogic.Interfaces;
using DataAccess.Data;
using ElectronicsRentTP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ElectronicsRentTP.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEquipmentService eq;

        public HomeController(IEquipmentService eq)
        {
            this.eq = eq;
        }

        public async Task<IActionResult> Index()
        {
            var equipment = await eq.GetAll(null, null, null, null, null,null,null, 1 );
            //var equipment = ctx.Equipments.Include(x => x.Category).ToList();
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
