using BusinessLogic.Interfaces;
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
        private readonly IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [Authorize]
        public async Task<IActionResult> MyMessages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vm = await chatService.GetMyMessages(userId!);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OpenWithOwner(int equipmentId)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Home");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (chatRoomId, exists) = await chatService.CreateOrGetChatRoom(equipmentId, userId!);

            if (!exists)
                return RedirectToAction("Details", "Equipment", new { id = equipmentId });

            return RedirectToAction("Room", new { chatRoomId = chatRoomId });
        }

        [HttpGet]
        public async Task<IActionResult> Room(int chatRoomId)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Home");

            var chatRoom = await chatService.GetChatRoom(chatRoomId);

            if (chatRoom == null)
                return NotFound();

            return View(chatRoom);
        }
        [Authorize]
        public async Task<IActionResult> Pin(int chatRoomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await chatService.PinUnpinChatRoom(chatRoomId, userId!, true);

            return RedirectToAction("MyMessages");
        }

        [Authorize]
        public async Task<IActionResult> Unpin(int chatRoomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await chatService.PinUnpinChatRoom(chatRoomId, userId!, false);

            return RedirectToAction("MyMessages");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int chatRoomId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await chatService.Delete(chatRoomId, userId!);
            return RedirectToAction("MyMessages");
        }

    }
}
