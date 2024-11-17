using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class MessageDTO(string content, UserDTO? user = null, DateTime? sendAt = null)
    {
        int? Id { get; set; }
        public string Content { get; set; } = content;
        public UserDTO? User { get; set; } = user;
        public DateTime SendAt { get; set; } = sendAt ?? DateTime.Now;
    }
}
