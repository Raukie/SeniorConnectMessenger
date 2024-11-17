using DataAccessLayer.DTO;

namespace SeniorConnectMessengerWeb.Models.DTO
{
    public class ChatPollDTO
    {
        public string Hash { get; set; }
        public int Id { get; set; }
        public int LastFetchedMessageId { get; set; }
        public bool IsOpen { get; set; }
    }
}
