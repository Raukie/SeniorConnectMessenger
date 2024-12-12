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
    public interface IChatUpdateStrategy
    {
        ChatUpdateDTO? CheckForUpdate(ChatPollDTO chatToPoll, List<Chat> chats, int userId, IChatStorage chatRepository);
    }
}
