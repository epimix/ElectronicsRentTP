using DataAccess.Data.Entities;
using DataAccess.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IRentalRepository
    {
        Task<Rental?> GetByIdAsync(int id, params Expression<Func<Rental, object>>[] includes);
        Task<Rental?> GetByIdForUpdateAsync(int id); // Без AsNoTracking для оновлення
        Task<List<Rental>> GetByUserIdAsync(string userId, int skip, int take);
        Task<int> CountOverlappingRentalsAsync(int equipmentId, DateTime startDate, DateTime endDate);
        Task AddAsync(Rental rental);
        void Update(Rental rental);
        void Delete(Rental rental);
        Task<bool> SaveChangesAsync();
        Task<List<Rental>> GetNotConfirmedRentalsAsync(string ownerId);
        Task<List<Rental>> GetExpiredActiveRentalsAsync(DateTime now); // для автоматичного завершення оренд
        Task<List<Rental>> GetRentalsByStatusAsync(string userId, RentalStatus status, int page, int pageSize = 10);

        Task<int> GetRentalCountByStatus(string userId, RentalStatus? status, int page, int pageSize = 10);

    }
}
