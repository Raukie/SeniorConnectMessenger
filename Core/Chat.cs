using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.Identity.Client;

namespace Core
{
    public class Chat
    {
        public Chat(string name, string hash, int id, MessageDTO lastReadMessage)
        {
            _name = name;
            _hash = hash;
            _id = id;
            messages.Add(lastReadMessage);
        }

        private int _id;
        private string _name;
        private string _hash;
        private List<MessageDTO> messages = new();
        private int? _lastReadMessageId { get; set; }
        public int? LastReadMessageId { get { return _lastReadMessageId; } }

        public int Id { get { return _id; } }
        public string Hash { get { return _hash; } }
        public string Name { get { return _name; } }

        public bool ShouldUpdateUI(string hash)
        {
            return hash != _hash;
        }

        public MessageDTO GetLastMessage()
        {
            return messages.Last();
        }

        public List<MessageDTO> GetUnreadMessagesInChat(ChatRepository chatRepository, int userId)
        {
            var unreadMessages = chatRepository.UpdateLastReadMessage(_id, userId);

            messages.AddRange(unreadMessages);

            return unreadMessages;
        }


    }
}
