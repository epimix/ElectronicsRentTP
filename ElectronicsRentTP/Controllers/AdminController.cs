using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsRentTP.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly EquipmentRentalDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IAdminService adminService;
        public AdminController(EquipmentRentalDbContext db, UserManager<User> userManager, IAdminService adminService)
        {
            _db = db;
            _userManager = userManager;
            this.adminService = adminService;
        }

        public async Task<IActionResult> Complaints()
        {
           var complaints = await adminService.GetComplaints();

            return View(complaints);
        }

        public async Task<IActionResult> Resolve(int id)
        {
            await adminService.ResolveComplaint(id);

            return RedirectToAction("Complaints");
        }

        public async Task<IActionResult> BanUser(string id)
        {
           await adminService.BanUser(id);

            return RedirectToAction("Complaints");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            await adminService.DeleteUser(id);

            return RedirectToAction("Complaints");
        }

        public async Task<IActionResult> DeleteAdvert(int id)
        {
            await adminService.DeleteAdvert(id);

            return RedirectToAction("Complaints");
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            await adminService.AddComment(id, comment);

            return RedirectToAction("Complaints");
        }
        public async Task<IActionResult> Refund(int id)
        {
            await adminService.Refound(id);

            return RedirectToAction("Complaints");
        }
    }
}
