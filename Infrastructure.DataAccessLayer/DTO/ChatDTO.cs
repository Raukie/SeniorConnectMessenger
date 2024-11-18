using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class ChatDTO
    {
        public ChatDTO(string name, List<UserDTO> users)
        {
            Name = name;
            Users = users;
        }
        public ChatDTO(string name)
        {
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserDTO> Users { get; set; } = new();
        public List<MessageDTO> Messages { get; set; } = new();
        public MessageDTO LastReadMessage { get; set; }
        public bool IsGroupChat { get; set; } 
        public string Hash { get; set; }

        /// <summary>
        /// Unmapped property for the frontend
        /// </summary>
        public int? AmountOfUnreadMessages { get; set; }
    }
}