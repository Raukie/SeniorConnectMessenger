using DataAccessLayer.DTO;
using System.Text.Json.Serialization;

namespace Core.Models.DTO
{
    public class ChatPollDTO
    {
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("lastFetchedMessageId")]
        public int LastFetchedMessageId { get; set; }
        [JsonPropertyName("isOpen")]
        public bool IsOpen { get; set; }
    }
}
