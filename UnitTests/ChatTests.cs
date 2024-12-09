using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitTests.MockDataAccess;

namespace UnitTests
{
    [TestClass]
    public class ChatTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var message = new MessageDTO("Hello", null, DateTime.Now) { Id = 1 };

            // Act
            var chat = new Core.Chat("Test Chat", "hash123", 1, message, 5);

            // Assert
            Assert.AreEqual(1, chat.Id);
            Assert.AreEqual("Test Chat", chat.Name);
            Assert.AreEqual("hash123", chat.Hash);
            Assert.AreEqual(1, chat.LastReadMessageId);
            Assert.AreEqual(5, chat.UnreadMessagesCount);
        }

        [TestMethod]
        public void ShouldUpdateUI_ShouldReturnTrue_WhenHashIsDifferent()
        {
            // Arrange
            var message = new MessageDTO("Hello", null, DateTime.Now) { Id = 1 };
            var chat = new Core.Chat("Test Chat", "hash123", 1, message, 0);

            // Act
            var result = chat.ShouldUpdateUI("differentHash");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldUpdateUI_ShouldReturnFalse_WhenHashIsSame()
        {
            // Arrange
            var message = new MessageDTO("Hello", null, DateTime.Now) { Id = 1 };
            var chat = new Core.Chat("Test Chat", "hash123", 1, message, 0);

            // Act
            var result = chat.ShouldUpdateUI("hash123");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetLastMessage_ShouldReturnLastMessage()
        {
            // Arrange
            var message = new MessageDTO("Hello", null, DateTime.Now) { Id = 1 };
            var chat = new Core.Chat("Test Chat", "hash123", 1, message, 0);

            // Act
            var lastMessage = chat.GetLastMessage();

            // Assert
            Assert.AreEqual("Hello", lastMessage.Content);
        }

        [TestMethod]
        public void GetUnreadMessagesInChat_ShouldReturnUnreadMessages()
        {
            // Arrange
            var mockStorage = new MockChatStorage();
            var chatDto = new ChatDTO
            {
                Id = 1,
                Messages = new List<MessageDTO>
                {
                    new MessageDTO("Message 1", null, DateTime.Now) { Id = 1 },
                    new MessageDTO("Message 2", null, DateTime.Now) { Id = 2 }
                },
                LastReadMessage = new MessageDTO("Message 1", null, DateTime.Now) { Id = 1 }
            };
            mockStorage.Chats.Add(chatDto);

            var initialMessage = new MessageDTO("Message 1", null, DateTime.Now) { Id = 1 };
            var chat = new Core.Chat("Test Chat", "hash123", 1, initialMessage, 1);

            // Act
            var unreadMessages = chat.GetUnreadMessagesInChat(mockStorage, 0);

            // Assert
            Assert.AreEqual(1, unreadMessages.Count);
            Assert.AreEqual("Message 2", unreadMessages[0].Content);
        }
    }
}
