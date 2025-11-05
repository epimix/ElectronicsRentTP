using DataAccess.Data.Entities;
using DataAccess.Data.Enum;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly EquipmentRentalDbContext _ctx;

        public RentalRepository(EquipmentRentalDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Rental?> GetByIdAsync(int id, params Expression<Func<Rental, object>>[] includes)
        {
            IQueryable<Rental> query = _ctx.Rentals.AsNoTracking();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Rental>> GetByUserIdAsync(string userId, int skip, int take)
        {
            return await _ctx.Rentals
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Include(r => r.Equipment)
                .OrderByDescending(r => r.StartDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountOverlappingRentalsAsync(int equipmentId, DateTime startDate, DateTime endDate)
        {
            return await _ctx.Rentals
                .Where(r => r.EquipmentId == equipmentId
                            && r.Status != RentalStatus.Cancelled
                            && r.Status != RentalStatus.Rejected
                            && r.EndDate >= startDate
                            && r.StartDate <= endDate)
                .CountAsync();
        }

        public async Task AddAsync(Rental rental)
        {
            await _ctx.Rentals.AddAsync(rental);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _ctx.SaveChangesAsync() > 0;
        }
        public async Task<List<Rental>> GetAllRentalsAsync()
        {
            return await _ctx.Rentals
                .AsNoTracking()
                .Include(r => r.Equipment)
                .Include(r => r.User)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }
        public void Update(Rental rental)
        {
            _ctx.Rentals.Update(rental);
        }

        public void Delete(Rental rental)
        {
            _ctx.Rentals.Remove(rental);
        }
    }
}
