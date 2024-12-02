using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorConnectMessengerWeb.Helpers;
using SeniorConnectMessengerWeb.Models.DTO;

namespace SeniorConnectMessengerWeb.Controllers
{
    [Authorize]
    public class ChatController(ChatRepository chatRepository, UserService userService, ChatService chatService): Controller
    {
        private ChatRepository _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
        private UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private ChatService _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));

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

        public IActionResult PollForUpdates(List<ChatPollDTO> chatsToPoll)
        {
            int userId = userService.GetCurrentUserId(HttpContext);
            var chats = _chatService.GetAllChatsUserIsIn(userId);
            List<ChatUpdateDTO> chatUpdatesDTO = new List<ChatUpdateDTO>();

            foreach (var chatToPoll in chatsToPoll) 
            {
                var chat = chats.FirstOrDefault(cht=>cht.Id == chatToPoll.Id);
                //chat has been removed
                if(chat == null)
                {
                    chatUpdatesDTO.Add(new ChatUpdateDTO() { Removed = true, Id = chatToPoll.Id });
                    continue;
                }
                ChatUpdateDTO chatUpdate = new() { Id = chat.Id };
                chatUpdate.Hash = chatToPoll.Hash;

                // if hash is different the name is altered or the users. Update name here
                if (chat.ShouldUpdateUI(chatToPoll.Hash))
                {
                    chatUpdate.Hash = chat.Hash;
                    chatUpdate.Name = chat.Name;
                }

                if (chatToPoll.IsOpen)
                {
                    chatUpdate.messages = chat.GetUnreadMessagesInChat(chatRepository, userId);
                } else if(chat.GetLastMessage().Id != chatToPoll.LastFetchedMessageId)
                {
                    chatUpdate.messages.Add(chat.GetLastMessage());
                }
            }

            return Json(chatUpdatesDTO);
        }
    }
}
