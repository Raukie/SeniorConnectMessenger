using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;

namespace SeniorConnectMessengerWeb.Controllers
{
    public class ChatController(ChatRepository chatRepository): Controller
    {
        private ChatRepository _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));

        public IActionResult GetUserChats()
        {
            return Json(new List<ChatDTO>());
        }
    }
}
