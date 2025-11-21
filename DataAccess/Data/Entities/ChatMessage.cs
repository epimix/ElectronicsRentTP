using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Entities
{
    public class ChatMessage : BaseEntity
    {
        public int Id { get; set; }

        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; }

        public string SenderId { get; set; }
        public User Sender { get; set; }

        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
