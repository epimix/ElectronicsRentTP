using DataAccess.Data.Enum;

namespace ElectronicsRentTP.Models
{
    public class RentalDetailsViewModel
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public RentalStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string? UserEmail { get; set; }
        public string? EquipmentImageUrl { get; set; }
    }
}
