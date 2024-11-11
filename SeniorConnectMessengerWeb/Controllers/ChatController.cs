using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using SeniorConnectMessengerWeb.Helpers;

namespace SeniorConnectMessengerWeb.Controllers
{
    public class ChatController(ChatRepository chatRepository, UserService userService): Controller
    {
        private ChatRepository _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
        private UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        public IActionResult GetUserChats()
        {
            int userId = userService.GetCurrentUserId(HttpContext);
            var chats = chatRepository.GetChatsUserIsIn(userId);

            return Json(chats);
        }

        public IActionResult GetChatContent(int chatId)
        {
            int userId = userService.GetCurrentUserId(HttpContext);
            var chat = chatRepository.GetChat(chatId);
            return Json(chat);
        }
    }
}
