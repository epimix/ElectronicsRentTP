using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;

namespace ElectronicsRentTP.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly EquipmentRentalDbContext ctx;

        public EquipmentController(EquipmentRentalDbContext ctx)
        {
            this.ctx = ctx;
        }

        public IActionResult Index()
        {
            // LEFT JOIN
            var model = ctx.Equipments.Include(x => x.Category).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var equipment = ctx.Equipments.Include(x => x.Category).FirstOrDefault(x => x.Id == id);
            if (equipment == null) return NotFound();

            return View(equipment);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SetCategoriesToViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Equipment equipment)
        {
            // Log ModelState errors for debugging
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var message in error.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Key: {error.Key}, Error: {message.ErrorMessage}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                ctx.Equipments.Add(equipment);
                ctx.SaveChanges();

                TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment created successfully!"));

                return RedirectToAction("Index");
            }

            SetCategoriesToViewBag();
            return View(equipment);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var equipment = ctx.Equipments.Find(id);
            if (equipment == null) return NotFound();

            SetCategoriesToViewBag();
            return View(equipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                ctx.Equipments.Update(equipment);
                ctx.SaveChanges();

                TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment updated successfully!"));

                return RedirectToAction("Index");
            }

            SetCategoriesToViewBag();
            return View(equipment);
        }

        public IActionResult Delete(int id)
        {
            var equipment = ctx.Equipments.Find(id);
            if (equipment == null) return NotFound();

            ctx.Equipments.Remove(equipment);
            ctx.SaveChanges(); // submit changes to DB

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
