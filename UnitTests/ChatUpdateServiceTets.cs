using core.Helpers;
using Core.Classes.ChatUpdate;
using Core.Models.DTO;
using Core.Services;
using DataAccessLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.MockDataAccess;

namespace UnitTests
{
    [TestClass]
    public class ChatUpdateServiceTests
    {
        private MockChatStorage _mockChatStorage;
        private ChatUpdateService _chatUpdateService;
        private MockUserStorage _mockUserStorage;
        private ChatService _chatService;

        [TestInitialize]
        public void Setup()
        {
            _mockChatStorage = new MockChatStorage();
            _mockUserStorage = new MockUserStorage();
            _chatService = new ChatService(_mockChatStorage, _mockUserStorage);
            _chatUpdateService = new ChatUpdateService(_mockChatStorage, _chatService);

            // Create mock users and chats
            var user = _mockUserStorage.CreateUser(new UserDTO("User1"), "password");
            var admin = _mockUserStorage.CreateUser(new UserDTO("Admin", null, true), "password");

            var chat = _mockChatStorage.CreateChat(new ChatDTO("Test Chat", new List<UserDTO> { user, admin }), admin);
            _mockChatStorage.CreateMessage(chat.Id, new MessageDTO("First Message", admin, DateTime.Now));
            _mockChatStorage.UpdateLastReadMessage(chat.Id, user.Id.Value);
        }

        [TestMethod]
        public void FetchChatUpdates_ShouldDetectRemovedChat()
        {
            // Arrange
            var user = _mockUserStorage.GetUserByUsername("User1");
            var chatsToPoll = new List<ChatPollDTO>
            {
                new ChatPollDTO { Id = 999, Hash = "oldHash" } // Non-existent chat
            };

            // Act
            var updates = _chatUpdateService.FetchChatUpdates(user.Id.Value, chatsToPoll);

            // Assert
            var removedChatUpdate = updates.FirstOrDefault(update => update.Removed);
            Assert.IsNotNull(removedChatUpdate);
            Assert.AreEqual(999, removedChatUpdate.Id);
            Assert.IsTrue(removedChatUpdate.Removed);
        }

        [TestMethod]
        public void FetchChatUpdates_ShouldDetectNewMessages()
        {
            // Arrange
            var user = _mockUserStorage.GetUserByUsername("User1");
            var chat = _mockChatStorage.GetChatsUserIsIn(user.Id.Value).First();
            var lastMessage = chat.Messages.Last();

            var chatsToPoll = new List<ChatPollDTO>
            {
                new ChatPollDTO { Id = chat.Id, LastFetchedMessageId = lastMessage.Id.Value , IsOpen = true}
            };

            // Add a new message to the chat
            _mockChatStorage.CreateMessage(chat.Id, new MessageDTO("New Message", user, DateTime.Now));

            // Act
            var updates = _chatUpdateService.FetchChatUpdates(user.Id.Value, chatsToPoll);

            // Assert
            var newMessageUpdate = updates.FirstOrDefault(update => update.messages?.Any(m => m.Content == "New Message") == true);
            Assert.IsNotNull(newMessageUpdate);
            Assert.AreEqual(chat.Id, newMessageUpdate.Id);
            Assert.AreEqual("New Message", newMessageUpdate.messages.First().Content);
        }

        [TestMethod]
        public void FetchChatUpdates_ShouldDetectUnreadMessagesWhenChatIsOpen()
        {
            // Arrange
            var user = _mockUserStorage.GetUserByUsername("User1");
            var chat = _mockChatStorage.GetChatsUserIsIn(user.Id.Value).First();

            var chatsToPoll = new List<ChatPollDTO>
            {
                new ChatPollDTO { Id = chat.Id, IsOpen = true, LastFetchedMessageId =  chat.LastReadMessage.Id.Value }
            };

            // Add unread messages
            _mockChatStorage.CreateMessage(chat.Id, new MessageDTO("Unread Message 1", user, DateTime.Now));
            _mockChatStorage.CreateMessage(chat.Id, new MessageDTO("Unread Message 2", user, DateTime.Now));

            // Act
            var updates = _chatUpdateService.FetchChatUpdates(user.Id.Value, chatsToPoll);

            // Assert
            var unreadMessageUpdate = updates.FirstOrDefault(update => update.messages?.Count == 2);
            Assert.IsNotNull(unreadMessageUpdate);
            Assert.AreEqual(2, unreadMessageUpdate.messages.Count);
        }

        [TestMethod]
        public void FetchChatUpdates_ShouldDetectUserOrNameChanges()
        {
            // Arrange
            var user = _mockUserStorage.GetUserByUsername("User1");
            var chat = _mockChatStorage.GetChatsUserIsIn(user.Id.Value).First();

            var chatsToPoll = new List<ChatPollDTO>
            {
                new ChatPollDTO { Id = chat.Id, Hash = "oldHash" }
            };

            // Update the chat name and hash
            chat.Name = "Updated Name";
            chat.Hash = "newHash";

            // Act
            var updates = _chatUpdateService.FetchChatUpdates(user.Id.Value, chatsToPoll);

            // Assert
            var userOrNameUpdate = updates.FirstOrDefault(update => update.Hash == "newHash");
            Assert.IsNotNull(userOrNameUpdate);
            Assert.AreEqual("newHash", userOrNameUpdate.Hash);
            Assert.AreEqual("Updated Name", userOrNameUpdate.Name);
        }

        [TestMethod]
        public void FetchChatUpdates_ShouldDetectNewChats()
        {
            // Arrange
            var user = _mockUserStorage.GetUserByUsername("User1");
            var chatsToPoll = new List<ChatPollDTO>(); // Simulate no chats on the front-end

            // Create a new chat and add the user
            var newChat = _mockChatStorage.CreateChat(new ChatDTO("New Chat", new List<UserDTO> { user }), user);

            // Act
            var updates = _chatUpdateService.FetchChatUpdates(user.Id.Value, chatsToPoll);

            // Assert
            var newChatUpdate = updates.LastOrDefault(update => update.IsNewChat);
            Assert.IsNotNull(newChatUpdate);
            Assert.AreEqual("New Chat", newChatUpdate.NewChat.Name);
        }
    }
}
