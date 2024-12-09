using Core;
using DataAccessLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.MockDataAccess;

namespace UnitTests
{
    [TestClass]
    public class GroupChatTests
    {
        [TestMethod]
        public void ShouldRemoveUserFromChat_WhenActorIsAdmin()
        {
            // Arrange
            var chatStorage = new MockChatStorage();
            var admin = new UserDTO("Admin", 1, true) { FirstName = "Admin", LastName = "User" };
            var user = new UserDTO("User", 2, false) { FirstName = "Regular", LastName = "User" };

            var groupChat = new GroupChat("Test Chat", "hash", 1, new MessageDTO("Welcome", admin, DateTime.Now), 0);
            chatStorage.CreateChat(new ChatDTO(groupChat.Name, new List<UserDTO> { admin, user }), admin);

            // Act
            var result = groupChat.RemoveUserFromChat(chatStorage, user, admin);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, chatStorage.GetChat(1, 1).Users.Count);
        }

        [TestMethod]
        public void ShouldNotRemoveUserFromChat_WhenActorIsNotAdmin()
        {
            // Arrange
            var chatStorage = new MockChatStorage();
            var nonAdmin = new UserDTO("NonAdmin", 3, false) { FirstName = "NonAdmin", LastName = "User" };
            var user = new UserDTO("User", 2, false) { FirstName = "Regular", LastName = "User" };

            var groupChat = new GroupChat("Test Chat", "hash", 1, new MessageDTO("Welcome", nonAdmin, DateTime.Now), 0);
            chatStorage.CreateChat(new ChatDTO(groupChat.Name, new List<UserDTO> { nonAdmin, user }), nonAdmin);

            // Act
            var result = groupChat.RemoveUserFromChat(chatStorage, user, nonAdmin);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(2, chatStorage.GetChat(1, 3).Users.Count);
        }

        [TestMethod]
        public void ShouldMakeUserAdmin_WhenActorIsAdmin()
        {
            // Arrange
            var chatStorage = new MockChatStorage();
            var admin = new UserDTO("Admin", 1, true) { FirstName = "Admin", LastName = "User" };
            var user = new UserDTO("User", 2, false);

            var groupChat = new GroupChat("Test Chat", "hash", 1, new MessageDTO("Welcome", admin, DateTime.Now), 0);
            chatStorage.CreateChat(new ChatDTO(groupChat.Name, new List<UserDTO> { admin, user }), admin);

            // Act
            var result = groupChat.MakeUserAdmin(chatStorage, user, admin.Id.Value);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(chatStorage.IsUserAdmin(1, 2));
        }

        [TestMethod]
        public void ShouldNotMakeUserAdmin_WhenActorIsNotAdmin()
        {
            // Arrange
            var chatStorage = new MockChatStorage();
            var nonAdmin = new UserDTO("NonAdmin", 3, false);
            var user = new UserDTO("User", 2, false);

            var groupChat = new GroupChat("Test Chat", "hash", 1, new MessageDTO("Welcome", nonAdmin, DateTime.Now), 0);
            chatStorage.CreateChat(new ChatDTO(groupChat.Name, new List<UserDTO> { nonAdmin, user }), nonAdmin);

            // Act
            var result = groupChat.MakeUserAdmin(chatStorage, user, nonAdmin.Id.Value);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(chatStorage.IsUserAdmin(1, 2));
        }

        [TestMethod]
        public void ShouldReturnTrue_WhenUserIsAdmin()
        {
            // Arrange
            var chatStorage = new MockChatStorage();
            var admin = new UserDTO("Admin", 1, true);

            var groupChat = new GroupChat("Test Chat", "hash", 1, new MessageDTO("Welcome", admin, DateTime.Now), 0);
            chatStorage.CreateChat(new ChatDTO(groupChat.Name, new List<UserDTO> { admin }), admin);

            // Act
            var result = groupChat.IsUserAdmin(chatStorage, admin.Id.Value);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldReturnFalse_WhenUserIsNotAdmin()
        {
            // Arrange
            var chatStorage = new MockChatStorage();
            var user = new UserDTO("User", 2, false);

            var groupChat = new GroupChat("Test Chat", "hash", 1, new MessageDTO("Welcome", user, DateTime.Now), 0);
            chatStorage.CreateChat(new ChatDTO(groupChat.Name, new List<UserDTO> { user }), user);

            // Act
            var result = groupChat.IsUserAdmin(chatStorage, user.Id.Value);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
