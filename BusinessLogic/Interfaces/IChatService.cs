using DataAccess.Data.Entities;
using DataAccess.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatListItemViewModel>> GetMyMessages(string userId);

        Task<ChatRoomViewModel?> GetChatRoomDetails(int chatRoomId, string userId);

        Task SendMessage(int chatRoomId, string senderId, string messageText);

        Task<(int?, bool)> CreateOrGetChatRoom(int equipmentId, string userId);

        Task PinUnpinChatRoom(int chatRoomId, string userId, bool isPinned);

        Task<ChatRoom?> GetChatRoom(int chatRoomId);

        Task Delete(int chatRoomId, string userId);
    }
}
