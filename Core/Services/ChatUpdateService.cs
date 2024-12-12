using core.Helpers;
using Core.Classes.ChatUpdate;
using Core.Models.DTO;
using Infrastructure.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class ChatUpdateService
    {
        private readonly IChatStorage _chatRepository;
        private readonly List<IChatUpdateStrategy> _strategies;
        private readonly ChatService _chatService;

        public ChatUpdateService(IChatStorage chatRepository, ChatService chatService)
        {
            _chatRepository = chatRepository;
            _strategies = new List<IChatUpdateStrategy>
            {
                new ChatUpdateStrategyChatRemoved(),
                new ChatUpdateStrategyUsersOrName(),
                new ChatUpdateStrategyOpenChat(),
                new ChatUpdateStrategyNewMessages()
            };
            _chatService = chatService;
        }

        public List<ChatUpdateDTO> FetchChatUpdates(int userId, List<ChatPollDTO> chatsToPoll)
        {
            var chats = GetAllChatsUserIsIn(userId);
            var chatUpdatesDTO = new List<ChatUpdateDTO>();

            foreach (var chatToPoll in chatsToPoll)
            {
                foreach (var strategy in _strategies)
                {
                    var chatUpdate = strategy.CheckForUpdate(chatToPoll, chats, userId, _chatRepository);
                    if (chatUpdate != null)
                    {
                        chatUpdatesDTO.Add(chatUpdate);
                        
                    }
                }
            }

            return chatUpdatesDTO;
        }

        private List<Chat> GetAllChatsUserIsIn(int userId)
        {
            return _chatService.GetAllChatsUserIsIn(userId);
        }
    }

}
