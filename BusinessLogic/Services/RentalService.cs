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

        public RentalService(IEquipmentService equipmentService, IRentalRepository rentalRepository)
        {
            _equipmentService = equipmentService;
            _rentalRepository = rentalRepository;
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

            var rental = new Rental
            {
                EquipmentId = equipment.Id,
                UserId = userId,
                StartDate = model.StartDate,
                Description = model.Description,
                PaymentType = model.PaymentType,
                EndDate = model.EndDate,
                Status = RentalStatus.Pending,
                TotalPrice = totalPrice
            };

            await _rentalRepository.AddAsync(rental);
            await _rentalRepository.SaveChangesAsync();

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
                r => r.User);

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
    }
}