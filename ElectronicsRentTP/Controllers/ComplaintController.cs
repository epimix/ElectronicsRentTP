using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Models;
using ElectronicsRentTP.Models;
using ElectronicsRentTP.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectronicsRentTP.Controllers
{
    [Authorize]
    public class ComplaintController : Controller
    {
        private readonly EquipmentRentalDbContext _db;

        public ComplaintController(EquipmentRentalDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public async Task<IActionResult> MyComplaints()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sent = await _db.Complaints
                .Where(c => c.ReporterId == userId)
                .Include(c => c.TargetUser)
                .ToListAsync();

            var againstMe = await _db.Complaints
                .Where(c => c.TargetUserId == userId)
                .Include(c => c.Reporter)
                .ToListAsync();

            var model = new MyComplaintsVM
            {
                SentComplaints = sent,
                ComplaintsAgainstMe = againstMe
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int rentalId)
        {
            var rental = await _db.Rentals
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

            if (rental == null)
                return NotFound();

            var model = new ComplaintCreateModel
            {
                RentalId = rentalId,
                TargetUserId = rental.OwnerId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ComplaintCreateModel model)
        {
            var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (reporterId == null)
                return Unauthorized();

            var complaint = new Complaint
            {
                ReporterId = reporterId,
                TargetUserId = model.TargetUserId,
                RentalId = model.RentalId,
                Message = model.Message,
                Created = DateTime.UtcNow
            };

            _db.Complaints.Add(complaint);
            await _db.SaveChangesAsync();

            return RedirectToAction("Success");
        }

        public IActionResult Success() => View();
    }
}
