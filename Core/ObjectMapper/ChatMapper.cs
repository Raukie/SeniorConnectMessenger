using DataAccessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ObjectMapper
{
    public static class ChatMapper
    {
        public static Chat ToChatDomain(this ChatDTO chatDto)
        {
            return new Chat(chatDto.Name, chatDto.Hash, chatDto.Id, chatDto.LastReadMessage, chatDto.AmountOfUnreadMessages);
        }

        public static GroupChat ToGroupChatDomain(this ChatDTO chatDto)
        {
            return new GroupChat(chatDto.Name, chatDto.Hash, chatDto.Id, chatDto.LastReadMessage, chatDto.AmountOfUnreadMessages);
        }
    }
}
