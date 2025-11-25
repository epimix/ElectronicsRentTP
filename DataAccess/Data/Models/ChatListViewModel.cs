using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data.Models
{
    public class ChatListItemViewModel
    {
        public int ChatRoomId { get; set; }

        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentImage { get; set; }

        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAvatar { get; set; }

        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }

        public bool IsPinned { get; set; }

        public bool IsMyMessage { get; set; }
    }
}
