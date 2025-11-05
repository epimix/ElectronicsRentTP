using DataAccess.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsRentTP.Models
{
    public class CreateRentalViewModel
    {
        public int EquipmentId { get; set; }

        public string? EquipmentName { get; set; }

        [Required]
        [Display(Name = "start date time")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "end date time")]
        public DateTime EndDate { get; set; }

        public string? Description { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }


        [Display(Name = "price for hour")]
        public decimal PricePerHour { get; set; }
    }
}
