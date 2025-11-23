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

        public AdminController(EquipmentRentalDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Complaints()
        {
            var complaints = await _db.Complaints
                .Include(c => c.Reporter)
                .Include(c => c.TargetUser)
                .Include(c => c.Rental)
                .Where(c => !c.Resolved)
                .OrderByDescending(c => c.Created)
                .ToListAsync();

            return View(complaints);
        }

        // ------------------- ACTIONS -------------------

        public async Task<IActionResult> Resolve(int id)
        {
            var complaint = await _db.Complaints.FirstOrDefaultAsync(c => c.Id == id);
            if (complaint == null) return NotFound();

            complaint.Resolved = true;
            await _db.SaveChangesAsync();

            return RedirectToAction("Complaints");
        }

        public async Task<IActionResult> BanUser(string id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.UtcNow.AddYears(50);

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Complaints");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);

            return RedirectToAction("Complaints");
        }

        public async Task<IActionResult> DeleteAdvert(int id)
        {
            var rental = await _db.Rentals.FindAsync(id);
            if (rental == null) return NotFound();

            rental.Status = RentalStatus.Cancelled;
            await _db.SaveChangesAsync();

            return RedirectToAction("Complaints");
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            complaint.AdminComment = comment;
            await _db.SaveChangesAsync();

            return RedirectToAction("Complaints");
        }
        public async Task<IActionResult> Refund(int id)
        {
            var rental = await _db.Rentals
                .Include(r => r.User)
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(r => r.Id == id);
               

            if (rental == null) return NotFound();


            rental.User.Balance += rental.TotalPrice;

            if(rental.Status != RentalStatus.Pending)
            {
                rental.Owner.Balance -= rental.TotalPrice;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Complaints");
        }
    }
}
