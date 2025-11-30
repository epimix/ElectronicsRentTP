using BusinessLogic.Interfaces;
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
        private readonly IComplaintService complaintService;

        public ComplaintController(IComplaintService complaintService)
        {
            this.complaintService = complaintService;
        }
        [Authorize]
        public async Task<IActionResult> MyComplaints()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await complaintService.GetMyComplaints(userId);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int rentalId)
        {
            var model = await complaintService.GetComplaintModel(rentalId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ComplaintCreateModel model)
        {
            var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (reporterId == null)
                return Unauthorized();

            await complaintService.Create(model, reporterId);

            return RedirectToAction("Success");
        }

        public IActionResult Success() => View();
    }
}
