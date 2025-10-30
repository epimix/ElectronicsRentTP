using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Models;
using BusinessLogic.Interfaces;

namespace ElectronicsRentTP.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly EquipmentRentalDbContext ctx;
        private readonly IEquipmentService eq;
        public EquipmentController(EquipmentRentalDbContext ctx, IEquipmentService eq)
        {
            this.ctx = ctx;
            this.eq = eq;
        }

        public async Task<IActionResult> Index()
        {
            // LEFT JOIN
            //var model = ctx.Equipments.Include(x => x.Category).ToList();
            var models = await eq.GetAll(null, null, null, null, null, null, null, 1);

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var equipment = await eq.GetById(id);
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
        public async Task<IActionResult> Create(Equipment equipment)
        {
            if (!ModelState.IsValid)
            {
                SetCategoriesToViewBag();
                return View(equipment);
            }

            await eq.AddEquipment(equipment);
            TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment created successfully!"));

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var equipment = await eq.GetById(id);
            if (equipment == null) return NotFound();

            SetCategoriesToViewBag();
            return View(equipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Equipment equipment)
        {
            if (!ModelState.IsValid)
            {
                SetCategoriesToViewBag();
                return View(equipment);
            }

            await eq.UpdateEquipment(equipment);
            TempData.Set(WebConstants.ToastMessage, new ToastModel("Equipment updated successfully!"));

            return RedirectToAction("Index");
        }


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
