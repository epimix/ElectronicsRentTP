using BusinessLogic.Interfaces;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ChatService : IChatService
    {
        private readonly EquipmentRentalDbContext ctx;
        public ChatService(EquipmentRentalDbContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<(int?, bool)> CreateOrGetChatRoom(int equipmentId, string userId)
        {
            var equipment = await ctx.Equipments
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(e => e.Id == equipmentId);

            if (equipment == null)
                return (null, false);
            if (equipment.OwnerId == userId)
                return (null, false);


            var existingChat = await ctx.ChatRooms
                .FirstOrDefaultAsync(cr =>
                    cr.EquipmentId == equipmentId &&
                    cr.OwnerId == equipment.OwnerId &&
                    cr.RenterId == userId);

            if (equipment.OwnerId == userId)
                return (null, false);


            if (existingChat != null)
                return (existingChat.Id, true);

            var newChat = new ChatRoom
            {
                EquipmentId = equipmentId,
                OwnerId = equipment.OwnerId,
                RenterId = userId,
                IsPinned = false,
                IsDeleted = false
            };
            ctx.ChatRooms.Add(newChat);
            await ctx.SaveChangesAsync();
            return (newChat.Id, true);
        }

        public async Task<ChatRoom?> GetChatRoom(int chatRoomId)
        {
            var chatRoom = await ctx.ChatRooms
                .Include(cr => cr.Equipment)
                .Include(cr => cr.Owner)
                .Include(cr => cr.Renter)
                .Include(cr => cr.Messages)
                .Include(c => c.Messages).ThenInclude(m => m.Sender)

                .FirstOrDefaultAsync(cr => cr.Id == chatRoomId);

            if (chatRoom == null)
                return null;

            return chatRoom;
        }

        public Task<ChatRoomViewModel?> GetChatRoomDetails(int chatRoomId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ChatListItemViewModel>> GetMyMessages(string userId)
        {
            var chats = await ctx.ChatRooms
                .Include(c => c.Equipment)
                .Include(c => c.Owner)
                .Include(c => c.Renter)
                .Include(c => c.Messages)
                .Where(c => !c.IsDeleted && (c.OwnerId == userId || c.RenterId == userId))
                .ToListAsync();

            var vm = chats.Select(c =>
            {
                var partner = c.OwnerId == userId ? c.Renter : c.Owner;

                var lastMsg = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();

                return new ChatListItemViewModel
                {
                    ChatRoomId = c.Id,
                    EquipmentId = c.EquipmentId,
                    EquipmentName = c.Equipment.Name,
                    EquipmentImage = c.Equipment.ImageUrl,

                    PartnerId = partner.Id,
                    PartnerName = partner.FullName ?? partner.Email,
                    PartnerAvatar = partner.profilePicture
                        ?? "https://cdn-icons-png.flaticon.com/512/149/149071.png",

                    LastMessage = lastMsg?.Text ?? "",
                    LastMessageTime = lastMsg?.SentAt,
                    IsMyMessage = lastMsg?.SenderId == userId,
                    IsPinned = c.IsPinned
                };
            }).OrderByDescending(c => c.IsPinned)
                .ThenByDescending(c => c.LastMessageTime)
                .ToList();

            return vm;
        }

        public async Task PinUnpinChatRoom(int chatRoomId, string userId, bool isPinned)
        {
            var chat = await ctx.ChatRooms
                .FirstOrDefaultAsync(c => c.Id == chatRoomId);

            if (chat == null)
                throw new Exception("Chat room not found.");

            if (chat.OwnerId != userId && chat.RenterId != userId)
                throw new UnauthorizedAccessException("You do not have permission to modify this chat room.");

            chat.IsPinned = isPinned;
            await ctx.SaveChangesAsync();
        }
        public async Task Delete(int chatRoomId, string userId)
        {
            var chat = await ctx.ChatRooms
                .FirstOrDefaultAsync(c => c.Id == chatRoomId);

            if (chat == null)
                throw new Exception("Chat room not found.");

            if (chat.OwnerId != userId && chat.RenterId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this chat room.");

            chat.IsDeleted = true;
            await ctx.SaveChangesAsync();

        }

        public Task SendMessage(int chatRoomId, string senderId, string messageText)
        {
            throw new NotImplementedException();
        }
    }
}
