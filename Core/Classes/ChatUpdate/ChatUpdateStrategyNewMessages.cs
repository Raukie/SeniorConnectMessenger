using Core.Models.DTO;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Classes.ChatUpdate
{
    public class ChatUpdateStrategyNewMessages : IChatUpdateStrategy
    {
        public ChatUpdateDTO? CheckForUpdate(ChatPollDTO chatToPoll, List<Chat> chats, int userId, IChatStorage chatRepository)
        {
            var chat = chats.FirstOrDefault(c => c.Id == chatToPoll.Id);
            if (chat == null || chatToPoll.IsOpen) return null;

            var lastMessage = chat.GetLastMessage();
            if (lastMessage.Id != chatToPoll.LastFetchedMessageId)
            {
                return new ChatUpdateDTO
                {
                    Id = chat.Id,
                    messages = new() { lastMessage },
                    AmountOfUnreadMessages = chat.UnreadMessagesCount
                };
            }

            return null;
        }
    }
}
