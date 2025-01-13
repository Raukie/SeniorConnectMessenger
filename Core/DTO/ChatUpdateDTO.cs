using DataAccessLayer.DTO;
using System.Text.Json.Serialization;

namespace Core.Models.DTO
{
    public class ChatUpdateDTO
    {
        public List<MessageDTO> messages { get; set; } = new List<MessageDTO>();
        public string Hash { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public bool Removed { get; set; }
        public int AmountOfUnreadMessages { get; set; }

        // only used for completely new chats
        public bool IsNewChat { get; set; } = false;
        public ChatDTO? NewChat { get; set; }
    }
}
