using DataAccess.Data;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsRentTP.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly EquipmentRentalDbContext _context;

        public ChatHub(EquipmentRentalDbContext context)
        {
            _context = context;
        }

        // Користувач підключається до кімнати
        public async Task JoinRoom(int chatRoomId)
        {
            var room = await _context.ChatRooms.FindAsync(chatRoomId);
            if (room == null)
                throw new HubException("Chat room not found");

            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        }

        // Відправка повідомлення
        public async Task SendMessage(int chatRoomId, string text)
        {
            var senderId = Context.UserIdentifier ??
                           Context.User?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(senderId))
                throw new HubException("User not authorized");

            var chatRoom = await _context.ChatRooms
                .Include(r => r.Owner)
                .Include(r => r.Renter)
                .FirstOrDefaultAsync(r => r.Id == chatRoomId);

            if (chatRoom == null)
                throw new HubException("Chat room not found");

            // Перевіряємо, чи юзер є учасником цього чату
            if (senderId != chatRoom.OwnerId && senderId != chatRoom.RenterId)
                throw new HubException("You are not a participant of this chat");

            var message = new ChatMessage
            {
                ChatRoomId = chatRoomId,
                SenderId = senderId,
                Text = text,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Розсилка всім, хто в групі
            await Clients.Group(chatRoomId.ToString())
                .SendAsync("ReceiveMessage", new
                {
                    chatRoomId,
                    senderId,
                    text,
                    sentAt = message.SentAt.ToString("O") // ISO 8601 формат
                });
        }
    }
}
