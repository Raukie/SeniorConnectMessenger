using core.Helpers;
using DataAccessLayer.DTO;
using UnitTests.MockDataAccess;

namespace UnitTests
{
    [TestClass]
    public class ChatServiceTests
    {
        [TestMethod]
        public void GetChatReturnsCorrectChat()
        {
            var chatStorage = new MockChatStorage();
            var userStorage = new MockUserStorage();

            var chatService = new ChatService(chatStorage);

            userStorage.CreateUser(new UserDTO("Ryan"), "fontys123");
            userStorage.CreateUser(new UserDTO("Peter"), "fontys123");

            chatStorage.Chats.Add(new ChatDTO("Coole groepschat", new() {
                new("Ryan", 1, true),
                new("Peter", 2, true)
            }));


            chatStorage.Chats.Add(new ChatDTO("Niet coole groepschat", new() {
                new("Ryan", 1, true),
                new("Peter", 2, true)
            }));

            var chat1 = chatService.GetChat(1, 1, false);
            var chat2 = chatService.GetChat(1, 1, false);

            Assert.Equals(chat1.Name, "Coole groepschat");
            Assert.Equals(chat2.Name, "Niet coole groepschat");
        }

        public void GetAllChatsUserIsInIsCorrect()
        {
            var chatStorage = new MockChatStorage();
            var userStorage = new MockUserStorage();

            var chatService = new ChatService(chatStorage);

            userStorage.CreateUser(new UserDTO("Ryan"), "fontys123");
            userStorage.CreateUser(new UserDTO("Peter"), "fontys123");
            userStorage.CreateUser(new UserDTO("Jan"), "fontys123");
            userStorage.CreateUser(new UserDTO("Bob"), "fontys123");

            chatStorage.Chats.Add(new ChatDTO("Coole groepschat", new() {
                new("Ryan", 1, true),
                new("Peter", 2, true),
                new("Jan", 3, true)
            }));


            chatStorage.Chats.Add(new ChatDTO("Niet coole groepschat", new() {
                new("Ryan", 1, true),
                new("Bob", 4, true),
            }));

            chatStorage.Chats.Add(new ChatDTO("Niet coole groepschat", new() {
                new("Bob", 4, true),
                new("Jan", 3, true)
            }));

            chatStorage.Chats.Add(new ChatDTO("Niet coole groepschat", new() {
                new("Ryan", 1, true),
                new("Jan", 3, true)
            }));

            chatStorage.Chats.Add(new ChatDTO("Niet coole groepschat", new() {
                new("Bob", 4, true),
                new("Peter", 2, true),
                new("Jan", 3, true)
            }));


            var chatsRyan = chatService.GetAllChatsUserIsIn(1);
            var chatsPeter = chatService.GetAllChatsUserIsIn(2);
            var chatsJan = chatService.GetAllChatsUserIsIn(3);
            Assert.Equals(3, chatsRyan.Count);
            Assert.Equals(2, chatsPeter.Count);
            Assert.Equals(4, chatsJan.Count);
        }

        [TestMethod]
        public void OnlyAdminCanCreateAdmins()
        {
            var chatStorage = new MockChatStorage();
            var userStorage = new MockUserStorage();

            var chatService = new ChatService(chatStorage);

            userStorage.CreateUser(new UserDTO("Ryan"), "fontys123");
            userStorage.CreateUser(new UserDTO("Peter"), "fontys123");

            var ryan = userStorage.GetUserById(1);
            var peter = userStorage.GetUserById(2);

            chatStorage.Chats.Add(new ChatDTO("Coole groepschat", new() {
                new("Ryan", 1, false),
                new("Peter", 2, true)
            }));


            chatStorage.Chats.Add(new ChatDTO("Niet coole groepschat", new() {
                new("Ryan", 1, true),
                new("Peter", 2, false)
            }));

            var chat1 = chatService.GetGroupChat(1, 1);
            var chat2 = chatService.GetGroupChat(2, 1);

            bool ryanCanCreateAdminsInChat1 = chat1.MakeUserAdmin(chatStorage, peter, ryan.Id.Value); 
            bool ryanCanCreateAdminsInChat2 = chat2.MakeUserAdmin(chatStorage, peter, ryan.Id.Value);

            bool peterCanCreateAdminsInChat1 = chat1.MakeUserAdmin(chatStorage, ryan, peter.Id.Value);
            bool peterCanCreateAdminsInChat2 = chat2.MakeUserAdmin(chatStorage, ryan, peter.Id.Value);

            Assert.IsTrue(!ryanCanCreateAdminsInChat1);
            Assert.IsTrue(ryanCanCreateAdminsInChat2);
            Assert.IsTrue(peterCanCreateAdminsInChat1);
            Assert.IsTrue(peterCanCreateAdminsInChat2); // peter should be admin now
        }
    }
}