using DataAccessLayer.DTO;
using DataAccessLayer.Extensions;
using Infrastructure.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.MockDataAccess
{
    internal class MockChatStorage : IChatStorage
    {
        internal List<ChatDTO> Chats { get; set; } = new();
        public ChatDTO CreateChat(ChatDTO chatDto, UserDTO userCreatedBy, DateTime? overrideDate)
        {
            Chats.Add(chatDto);
            var newChat = Chats.Last();
            newChat.Id = Chats.Count();
            CreateMessage(newChat.Id, new MessageDTO($"{userCreatedBy.FirstName} {userCreatedBy.LastName} heeft de chat aangemaakt", null, DateTime.Now));
            newChat.Hash = newChat.CalculateHash();
            newChat.LastReadMessage = newChat.Messages.Last();
            newChat.AmountOfUnreadMessages = 1;
            return newChat;
        }

        public void CreateMessage(int chatId, MessageDTO message)
        {
            var chat = Chats.FirstOrDefault(chat=>chat.Id == chatId);
            message.Id = chat.Messages.Count + 1;
            chat.Messages.Add(message);
        }

        public ChatDTO GetChat(int chatId, int userId, bool updateLastReadMessage = false)
        {
            var chat = Chats.FirstOrDefault(chat=> chat.Id == chatId);
            if (updateLastReadMessage)
            {
                chat.LastReadMessage = chat.Messages.LastOrDefault();
                chat.AmountOfUnreadMessages = 0;
            }
            return chat;
        }

        public List<ChatDTO> GetChatsUserIsIn(int userId)
        {
            return Chats.Where(chat => chat.Users.Any(user => user.Id == userId)).ToList();
        }

        public bool IsUserAdmin(int chatId, int userId)
        {
            var chat = Chats.FirstOrDefault(chat => chat.Id == chatId);
            return chat.Users.Any(user => user.Id == userId && user.IsAdmin == true);
        }

        public bool MakeUserAdmin(int chatId, int userId)
        {
            var chat = Chats.FirstOrDefault(chat => chat.Id == chatId);
            if(!chat.Users.Any(user=>user.Id == userId))
            {
                return false;
            }

            chat.Users.First(user => user.Id == userId).IsAdmin = true;
            return true;
        }

        public bool RemoveUserFromChat(int chatId, int userId)
        {
            var chat = Chats.FirstOrDefault(chat => chat.Id == chatId);
            chat.Users.Remove(chat.Users.First(user=>user.Id == userId));
            return true;
        }

        public List<MessageDTO> UpdateLastReadMessage(int chatId, int userId)
        {
            var chat = Chats.FirstOrDefault(chat => chat.Id == chatId);
            var unreadMessages = chat.Messages.Where(message => message.Id > chat.LastReadMessage.Id).ToList();
            chat.LastReadMessage = chat.Messages.LastOrDefault();
            return unreadMessages.ToList();
        }
    }
}
