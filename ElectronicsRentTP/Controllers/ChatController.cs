using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectronicsRentTP.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly EquipmentRentalDbContext _context;

        public ChatController(EquipmentRentalDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> MyMessages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chats = await _context.ChatRooms
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
                    IsMyMessage = lastMsg?.SenderId == userId
                };
            }).OrderByDescending(c => c.IsPinned)
    .ThenByDescending(c => c.LastMessageTime)
    .ToList();

            return View(vm);
        }




        [HttpGet]
        public async Task<IActionResult> OpenWithOwner(int equipmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var equipment = await _context.Equipments
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(e => e.Id == equipmentId);

            if (equipment == null)
                return NotFound();


            if (equipment.OwnerId == userId)
            {

                return RedirectToAction("Details", "Equipment", new { id = equipmentId });
            }


            var chatRoom = await _context.ChatRooms
                .FirstOrDefaultAsync(cr =>
                    cr.EquipmentId == equipmentId &&
                    cr.OwnerId == equipment.OwnerId &&
                    cr.RenterId == userId);

            if (chatRoom == null)
            {
                chatRoom = new ChatRoom
                {
                    EquipmentId = equipmentId,
                    OwnerId = equipment.OwnerId!,
                    RenterId = userId!
                };

                _context.ChatRooms.Add(chatRoom);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Room", new { chatRoomId = chatRoom.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Room(int chatRoomId)
        {
            var chatRoom = await _context.ChatRooms
                .Include(cr => cr.Equipment)
                .Include(cr => cr.Owner)
                .Include(cr => cr.Renter)
                .Include(cr => cr.Messages)
                .Include(c => c.Messages).ThenInclude(m => m.Sender)

                .FirstOrDefaultAsync(cr => cr.Id == chatRoomId);

            if (chatRoom == null)
                return NotFound();


            return View(chatRoom);
        }
        [Authorize]
        public async Task<IActionResult> Pin(int chatRoomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chat = await _context.ChatRooms
                .FirstOrDefaultAsync(c => c.Id == chatRoomId);

            if (chat == null)
                return NotFound();

            if (chat.OwnerId != userId && chat.RenterId != userId)
                return Forbid();

            chat.IsPinned = true;

            await _context.SaveChangesAsync();
            return RedirectToAction("MyMessages");
        }

        [Authorize]
        public async Task<IActionResult> Unpin(int chatRoomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chat = await _context.ChatRooms
                .FirstOrDefaultAsync(c => c.Id == chatRoomId);

            if (chat == null)
                return NotFound();

            if (chat.OwnerId != userId && chat.RenterId != userId)
                return Forbid();

            chat.IsPinned = false;

            await _context.SaveChangesAsync();
            return RedirectToAction("MyMessages");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int chatRoomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chat = await _context.ChatRooms
                .FirstOrDefaultAsync(c => c.Id == chatRoomId);

            if (chat == null)
                return NotFound();

            if (chat.OwnerId != userId && chat.RenterId != userId)
                return Forbid();

            chat.IsDeleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction("MyMessages");
        }

    }
}
