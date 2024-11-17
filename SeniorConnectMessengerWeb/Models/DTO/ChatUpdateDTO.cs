using DataAccessLayer.DTO;

namespace SeniorConnectMessengerWeb.Models.DTO
{
    public class ChatUpdateDTO
    {
        public List<MessageDTO> messages = new List<MessageDTO>();
        public string Hash { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public bool Removed { get; set; }
    }
}
