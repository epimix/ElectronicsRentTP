using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;

namespace ElectronicsRentTP.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly EquipmentRentalDbContext ctx;

        public FavoriteController(EquipmentRentalDbContext ctx)
        {
            this.ctx = ctx;
        }
        
        // GET: Cart
        public async Task<ActionResult> Index()
        {
            var existingIds = HttpContext.Session.Get<List<int>>("FavItems") ?? new List<int>();

            var items = await ctx.Equipments
                .Include(x => x.Category)
                .Include(x => x.reviews)
                .Where(x => existingIds.Contains(x.Id))
                .ToListAsync();

            return View(items);
        }

        public ActionResult Add(int id) // 3, 5
        {
            var existingIds = HttpContext.Session.Get<List<int>>("FavItems");
            List<int> ids = existingIds ?? new();
            ids.Add(id);

            HttpContext.Session.Set("FavItems", ids);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Delete(int id)
        {
            var existingIds = HttpContext.Session.Get<List<int>>("FavItems");
            List<int> ids = existingIds ?? new();

            ids.Remove(id);
            HttpContext.Session.Set("FavItems", ids);

            TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment removed from favorites successfully!"));

            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll()
        {
            var existingIds = HttpContext.Session.Get<List<int>>("FavItems");
            List<int> ids = existingIds ?? new();

            ids.Clear();
            HttpContext.Session.Set("FavItems", ids);

            TempData.Set(WebConstants.ToastMessage, new ToastModel("All items removed from favorites successfully!"));

            return RedirectToAction("Index");
        }
    }
}
