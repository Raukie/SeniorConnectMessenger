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
    public class ChatUpdateStrategyUsersOrName : IChatUpdateStrategy
    {
        public ChatUpdateDTO? CheckForUpdate(ChatPollDTO chatToPoll, List<Chat> chats, int userId, IChatStorage chatRepository)
        {
            var chat = chats.FirstOrDefault(c => c.Id == chatToPoll.Id);
            if (chat == null) return null;

            if (chat.ShouldUpdateUI(chatToPoll.Hash))
            {
                return new ChatUpdateDTO
                {
                    Id = chat.Id,
                    Hash = chat.Hash,
                    Name = chat.Name
                };
            }

            return null;
        }
    }

}

