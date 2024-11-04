using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class ChatDTO(string name, List<UserDTO> users, List<MessageDTO> messages, int lastReadMessageID, string hash)
    {
        public string Name { get; set; } = name;
        public List<UserDTO> Users { get; set; } = users;
        public List<MessageDTO> Messages { get; set; } = messages;
        public int LastReadMessageID { get; set; } = lastReadMessageID;
        public string Hash { get; set; } = hash;
    }
}