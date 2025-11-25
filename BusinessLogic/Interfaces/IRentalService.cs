using DataAccess.Data.Entities;
using DataAccess.Data.Enum;
using ElectronicsRentTP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IRentalService
    {
        Task<CreateRentalViewModel?> InitializeRentalAsync(int equipmentId);

        Task<(bool Success, string ErrorMessage, Rental? Rental)> CreateRentalAsync(CreateRentalViewModel model, string userId);
        Task<(string Name, decimal PricePerHour)> GetEquipmentDetailsAsync(int equipmentId);

        Task<List<RentalListItemViewModel>> GetUserRentalsAsync(string userId, int? page = 1, int? pageSize = 10);
        Task<RentalDetailsViewModel?> GetRentalDetailsAsync(int id, string currentUserId);
        Task<(bool Success, string ErrorMessage)> UpdateRentalAsync(int id, string currentUserId, Action<Rental> updateAction);
        Task<(bool Success, string ErrorMessage)> DeleteRentalAsync(int id, string currentUserId);

        Task ConfirmRental(int rentalId, string? ownerId = null);
        Task CancelRental(int rentalId); // для відміни замовником
        Task RejectRental(int rentalId); // для відміни продавцем

        Task AutoCompleteRentalsAsync(); // Автоматичне завершення оренд
        Task<IList<RentalDetailsViewModel>> GetNotConfirmRental(string userId);
        Task<IList<RentalDetailsViewModel>> GetRentalByStatus(string userId, RentalStatus status, int page, int pageSize = 10);
        Task<int> GetRentalCountByStatus(string userId, RentalStatus? status, int page, int pageSize = 10);
    }
}
