using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.Identity.Client;
using System.Reflection;

namespace Core
{
    public class Chat
    {
        public Chat(string name, string hash, int id, MessageDTO? lastReadMessage, int? unreadMessagesCount)
        {
            _name = name;
            _hash = hash;
            _id = id;
            if(lastReadMessage != null)
            {
                _lastReadMessageId = lastReadMessage.Id;
                messages.Add(lastReadMessage);
            }
            _unreadMessagesCount = unreadMessagesCount ?? 0;
		}

        protected int _id;
        protected string _name;
        protected string _hash;
        protected List<MessageDTO> messages = new();
        protected int _unreadMessagesCount { get; set; }
        protected int? _lastReadMessageId { get; set; }
        public int? LastReadMessageId { get { return _lastReadMessageId; } }
        public int UnreadMessagesCount { get { return _unreadMessagesCount; } }
        public int Id { get { return _id; } }
        public string Hash { get { return _hash; } }
        public string Name { get { return _name; } }

        public bool SendMessage(IChatStorage chatStorage, UserDTO user, string messageContent) 
        {
            try
            {
                chatStorage.CreateMessage(_id, new MessageDTO(messageContent) {User = user});
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ShouldUpdateUI(string hash)
        {
            return hash != _hash;
        }

        public MessageDTO GetLastMessage()
        {
            return messages.Last();
        }

        public List<MessageDTO> GetUnreadMessagesInChat(IChatStorage chatRepository, int userId)
        {
            var unreadMessages = chatRepository.UpdateLastReadMessage(_id, userId);

            messages.AddRange(unreadMessages);

            return unreadMessages;
        }


        public bool UserLeaveChat(IChatStorage chatRepository, UserDTO user)
        {
            chatRepository.CreateMessage(_id,
                new MessageDTO($"{user.FirstName} {user.LastName} heeft de chat verlaten")
            );

            return chatRepository.RemoveUserFromChat(_id, user.Id!.Value);
        }

        public IReadOnlyCollection<MessageDTO> GetMessages()
        {
            return messages;
        }
    }
}
