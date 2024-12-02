using Core;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using System.Linq;

namespace SeniorConnectMessengerWeb.Helpers
{
    public class ChatService(ChatRepository chatRepository)
    {
        private ChatRepository _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
        public List<Chat> GetAllChatsUserIsIn(int userId)
        {
            var chats = chatRepository.GetChatsUserIsIn(userId);

            return chats.Select(chat => new Chat(chat.Name, chat.Hash, chat.Id)).ToList();
        }
    }
}
