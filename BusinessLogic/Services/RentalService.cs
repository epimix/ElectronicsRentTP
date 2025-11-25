using BusinessLogic.Interfaces;
using ElectronicsRentTP.Models;
using DataAccess.Data.Entities;
using DataAccess.Data.Enum;
using DataAccess.Repositories;

namespace BusinessLogic.Services
{
    public class RentalService : IRentalService
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IRentalRepository _rentalRepository;
        private readonly IBalanceService _balanceService;

        public RentalService(IEquipmentService equipmentService, IRentalRepository rentalRepository, IBalanceService balanceService)
        {
            _equipmentService = equipmentService;
            _rentalRepository = rentalRepository;
            _balanceService = balanceService;
        }

        public async Task<CreateRentalViewModel?> InitializeRentalAsync(int equipmentId)
        {
            var equipment = await _equipmentService.GetById(equipmentId);

            if (equipment == null)
                return null;
            if (equipment.Status is EquipmentStatus.Rented or EquipmentStatus.Reserved || !equipment.IsAvailable)
                return null;
            return new CreateRentalViewModel
            {
                EquipmentId = equipment.Id,
                EquipmentName = equipment.Name,
                PricePerHour = equipment.PricePerHour,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1)
            };
        }

        public async Task<(string Name, decimal PricePerHour)> GetEquipmentDetailsAsync(int equipmentId)
        {
            var equipment = await _equipmentService.GetById(equipmentId);
            if (equipment == null)
                throw new InvalidOperationException("Equipment not found");

            return (equipment.Name, equipment.PricePerHour);
        }

        public async Task<(bool Success, string ErrorMessage, Rental? Rental)> CreateRentalAsync(CreateRentalViewModel model, string userId)
        {
            var equipment = await _equipmentService.GetById(model.EquipmentId);
            if (equipment == null)
                return (false, "can't find an equipment", null);

            if (equipment.Status is EquipmentStatus.Rented or EquipmentStatus.Reserved || !equipment.IsAvailable || equipment.Quantity <= 0)
                return (false, "This equipment is currently not available.", null);

            var overlappingCount = await _rentalRepository.CountOverlappingRentalsAsync(
                equipment.Id, model.StartDate, model.EndDate);

            if (overlappingCount >= equipment.Quantity)
                return (false, "this dates can't be booked.", null);

            var totalHours = (decimal)(model.EndDate - model.StartDate).TotalHours;
            if (totalHours <= 0) totalHours = 24m;

            var totalPrice = Math.Round(totalHours * equipment.PricePerHour, 2);

            var userBalance = await _balanceService.GetUserBalanceAsync(userId);
            if (userBalance < totalPrice)
                return (false, "Insufficient balance to rent this equipment.", null);

            await _balanceService.Payment(userId, totalPrice);

            // Якщо товар з БД (OwnerId == null або адмін), автоматично підтверджуємо
            var isFromDatabase = string.IsNullOrEmpty(equipment.OwnerId) || equipment.OwnerId == "551b73c1-3601-490c-90bf-5af17a4408d5";
            var initialStatus = isFromDatabase ? RentalStatus.Approved : RentalStatus.Pending;

            var rental = new Rental
            {
                EquipmentId = equipment.Id,
                UserId = userId,
                OwnerId = equipment.OwnerId,
                StartDate = model.StartDate,
                Description = model.Description,
                PaymentType = model.PaymentType,
                EndDate = model.EndDate,
                Status = initialStatus,
                TotalPrice = totalPrice
            };

            await _rentalRepository.AddAsync(rental);
            await _rentalRepository.SaveChangesAsync();

            // Якщо автоматично підтверджено, зменшуємо кількість
            if (isFromDatabase && equipment.Quantity > 0)
            {
                equipment.Quantity -= 1;
                if (equipment.Quantity == 0)
                {
                    equipment.IsAvailable = false;
                }
                await _equipmentService.UpdateEquipment(equipment);

                // Виплачуємо власнику (якщо є)
                if (!string.IsNullOrEmpty(equipment.OwnerId))
                {
                    await _balanceService.OwnerPay(equipment.OwnerId, totalPrice);
                }
            }

            return (true, string.Empty, rental);
        }

        public async Task<List<RentalListItemViewModel>> GetUserRentalsAsync(string userId, int? page = 1, int? pageSize = 10)
        {
            var skip = ((page ?? 1) - 1) * (pageSize ?? 10);
            var rentals = await _rentalRepository.GetByUserIdAsync(userId, skip, pageSize ?? 10);

            return rentals.Select(r => new RentalListItemViewModel
            {
                Id = r.Id,
                EquipmentId = r.EquipmentId,
                EquipmentName = r.Equipment.Name,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Status = r.Status,
                TotalPrice = r.TotalPrice
            }).ToList();
        }

        public async Task<RentalDetailsViewModel?> GetRentalDetailsAsync(int id, string currentUserId)
        {
            var rental = await _rentalRepository.GetByIdAsync(id,
                r => r.Equipment,
                r => r.User,
                r => r.Owner);

            if (rental == null || rental.UserId != currentUserId)
                return null;

            return new RentalDetailsViewModel
            {
                Id = rental.Id,
                EquipmentId = rental.EquipmentId,
                EquipmentName = rental.Equipment?.Name ?? "—",
                StartDate = rental.StartDate,
                EndDate = rental.EndDate,
                Status = rental.Status,
                TotalPrice = rental.TotalPrice,
                UserEmail = rental.User?.Email,
                OwnerEmail = rental.Owner?.Email,
                Description = rental.Description,
                PaymentType = rental.PaymentType,
                EquipmentImageUrl = rental.Equipment?.ImageUrl
            };
        }
        public async Task<(bool Success, string ErrorMessage)> UpdateRentalAsync(int id, string currentUserId, Action<Rental> updateAction)
        {
            var rental = await _rentalRepository.GetByIdAsync(id);

            if (rental == null)
                return (false, "Rental not found.");

            if (rental.UserId != currentUserId)
                return (false, "You don't have permission to update this rental.");

            if (rental.Status is RentalStatus.Completed or RentalStatus.Cancelled)
                return (false, "Cannot update completed or cancelled rental.");

            updateAction(rental);
            _rentalRepository.Update(rental);

            var saved = await _rentalRepository.SaveChangesAsync();
            return saved
                ? (true, string.Empty)
                : (false, "Failed to save changes.");
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteRentalAsync(int id, string currentUserId)
        {
            var rental = await _rentalRepository.GetByIdAsync(id);

            if (rental == null)
                return (false, "Rental not found.");

            if (rental.UserId != currentUserId)
                return (false, "You don't have permission to delete this rental.");

            if (rental.Status != RentalStatus.Pending)
                return (false, "Only pending rentals can be deleted.");

            _rentalRepository.Delete(rental);

            var saved = await _rentalRepository.SaveChangesAsync();
            return saved
                ? (true, string.Empty)
                : (false, "Failed to delete rental.");
        }
        public async Task ConfirmRental(int rentalId, string? ownerId = null)
        {
            // Завантажуємо Rental БЕЗ AsNoTracking для можливості оновлення
            var rental = await _rentalRepository.GetByIdForUpdateAsync(rentalId);
            if (rental == null)
                throw new InvalidOperationException("Rental not found.");

            // Перевіряємо, чи користувач має право підтверджувати цей rental
            if (!string.IsNullOrEmpty(ownerId) && rental.OwnerId != ownerId)
                throw new UnauthorizedAccessException("You don't have permission to approve this rental.");

            await _balanceService.OwnerPay(rental.OwnerId ?? "", rental.TotalPrice);
            rental.Status = RentalStatus.Approved;

            // Зменшуємо кількість товару - завантажуємо окремо щоб уникнути tracking конфлікту
            var equipment = await _equipmentService.GetById(rental.EquipmentId);
            if (equipment != null && equipment.Quantity > 0)
            {
                equipment.Quantity -= 1;
                if (equipment.Quantity == 0)
                {
                    equipment.IsAvailable = false;
                }
                await _equipmentService.UpdateEquipment(equipment);
            }

            _rentalRepository.Update(rental);
            await _rentalRepository.SaveChangesAsync();
        }
        public async Task CancelRental(int id)
        {
            var rental = await _rentalRepository.GetByIdAsync(id);
            if (rental == null)
                throw new InvalidOperationException("Rental not found");

            if (rental.Status != RentalStatus.Pending)
                throw new InvalidOperationException("Only pending rentals can be cancelled.");

            rental.Status = RentalStatus.Cancelled;
            _rentalRepository.Update(rental);

            await _balanceService.ReplenishmentBalanceAsync(rental.UserId, rental.TotalPrice);

            var saved = await _rentalRepository.SaveChangesAsync();

        }
        public async Task<IList<RentalDetailsViewModel>> GetNotConfirmRental(string userId)
        {
            var rentals = await _rentalRepository.GetNotConfirmedRentalsAsync(userId);

            return rentals.Select(r => new RentalDetailsViewModel
            {
                Id = r.Id,
                EquipmentId = r.EquipmentId,
                EquipmentName = r.Equipment?.Name ?? "—",
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Status = r.Status,
                TotalPrice = r.TotalPrice,
                UserEmail = r.User?.Email,
                OwnerEmail = r.Owner?.Email,
                Description = r.Description,
                PaymentType = r.PaymentType,
                EquipmentImageUrl = r.Equipment?.ImageUrl
            }).ToList();
        }

        public async Task RejectRental(int rentalId)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental != null)
            {
                rental.Status = RentalStatus.Rejected;
                _rentalRepository.Update(rental);
                await _balanceService.ReplenishmentBalanceAsync(rental.UserId, rental.TotalPrice);
                await _rentalRepository.SaveChangesAsync();
            }
        }

        public async Task AutoCompleteRentalsAsync()
        {
            var now = DateTime.UtcNow;

            var rentals = await _rentalRepository.GetExpiredActiveRentalsAsync(now);

            foreach (var rental in rentals)
            {
                rental.Status = RentalStatus.Completed;
                _rentalRepository.Update(rental);
            }
            if (rentals.Count > 0)
                await _rentalRepository.SaveChangesAsync();
        }
        public async Task<IList<RentalDetailsViewModel>> GetRentalByStatus(string userId, RentalStatus status, int page, int pageSize = 10)

        {
            var rentals = await _rentalRepository.GetRentalsByStatusAsync(userId, status, page, pageSize);

            return rentals.Select(r => new RentalDetailsViewModel
            {
                Id = r.Id,
                EquipmentId = r.EquipmentId,
                EquipmentName = r.Equipment?.Name ?? "—",
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                Status = r.Status,
                TotalPrice = r.TotalPrice,
                UserEmail = r.User?.Email,
                OwnerEmail = r.Owner?.Email,
                Description = r.Description,
                PaymentType = r.PaymentType,
                EquipmentImageUrl = r.Equipment?.ImageUrl
            }).ToList();

        }
        public async Task<int> GetRentalCountByStatus(string userId, RentalStatus? status, int page, int pageSize = 10)
        {
            return await _rentalRepository.GetRentalCountByStatus(userId, status, page, pageSize);
        }

    }
}