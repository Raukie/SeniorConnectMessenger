using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class ChatDTO(string name, List<UserDTO> users)
    {
        public int Id { get; set; }
        public string Name { get; set; } = name;
        public List<UserDTO> Users { get; set; } = users;
        public List<MessageDTO> Messages { get; set; }
        public int LastReadMessageID { get; set; }
        public bool IsGroupChat { get; set; } 
        public string Hash { get; set; }
    }
}