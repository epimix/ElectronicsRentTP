using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Models;
using ElectronicsRentTP.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly EquipmentRentalDbContext ctx;
        public ComplaintService(EquipmentRentalDbContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task Create(ComplaintCreateModel model, string reporterId)
        {
            var complaint = new Complaint
            {
                ReporterId = reporterId,
                TargetUserId = model.TargetUserId,
                RentalId = model.RentalId,
                Message = model.Message,
                Created = DateTime.UtcNow
            };

            ctx.Complaints.Add(complaint);
            await ctx.SaveChangesAsync();
        }

        public async Task<ComplaintCreateModel?> GetComplaintModel(int rentalId)
        {
            var rental = await ctx.Rentals
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

            if (rental == null)
                throw new NotImplementedException();

            var model = new ComplaintCreateModel
            {
                RentalId = rentalId,
                TargetUserId = rental.OwnerId
            };

            return model;

        }

        public async Task<MyComplaintsVM?> GetMyComplaints(string userId)
        {
            var sent = await ctx.Complaints
                            .Where(c => c.ReporterId == userId)
                            .Include(c => c.TargetUser)
                            .ToListAsync();

            var againstMe = await ctx.Complaints
                .Where(c => c.TargetUserId == userId)
                .Include(c => c.Reporter)
                .ToListAsync();

            var model = new MyComplaintsVM
            {
                SentComplaints = sent,
                ComplaintsAgainstMe = againstMe
            };

            return model;
        }
    }
}
