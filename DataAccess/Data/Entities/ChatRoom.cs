namespace DataAccess.Data.Entities
{
    public class ChatRoom : BaseEntity
    {
        public int Id { get; set; }

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public string OwnerId { get; set; } = null!;
        public User Owner { get; set; }

        public string RenterId { get; set; } = null!;
        public User Renter { get; set; }

        public bool IsPinned { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
