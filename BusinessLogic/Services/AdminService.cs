using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AdminService : IAdminService
    {
        private readonly EquipmentRentalDbContext _db;
        private readonly UserManager<User> _userManager;

        public AdminService(EquipmentRentalDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task AddComment(int id, string comment)
        {
            var complaint = await _db.Complaints.FindAsync(id);
            if (complaint == null)
                throw new KeyNotFoundException("Complaint not found");

            complaint.AdminComment = comment;
            await _db.SaveChangesAsync();
        }

        public async Task BanUser(string id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.UtcNow.AddYears(50);

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAdvert(int id)
        {
            var rental = await _db.Rentals.FindAsync(id);
            if (rental == null)
                throw new KeyNotFoundException("Rental not found");

            rental.Status = RentalStatus.Cancelled;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            await _userManager.DeleteAsync(user);
        }

        public async Task<List<Complaint>> GetComplaints()
        {
            var complaints = await _db.Complaints
               .Include(c => c.Reporter)
               .Include(c => c.TargetUser)
               .Include(c => c.Rental)
               .Where(c => !c.Resolved)
               .OrderByDescending(c => c.Created)
               .ToListAsync();

            return complaints;
        }

        public async Task Refound(int id)
        {
            var rental = await _db.Rentals
                .Include(r => r.User)
                .Include(r => r.Owner)
                .FirstOrDefaultAsync(r => r.Id == id);


            if (rental == null)
                throw new KeyNotFoundException("Rental not found");


            rental.User.Balance += rental.TotalPrice;

            if (rental.Status != RentalStatus.Pending)
            {
                rental.Owner.Balance -= rental.TotalPrice;
            }

            await _db.SaveChangesAsync();
        }

        public async Task ResolveComplaint(int id)
        {
            var complaint = await _db.Complaints.FirstOrDefaultAsync(c => c.Id == id);
            if (complaint == null) 
                throw new KeyNotFoundException("Complaint not found");

            complaint.Resolved = true;
            await _db.SaveChangesAsync();
        }
    }
}
