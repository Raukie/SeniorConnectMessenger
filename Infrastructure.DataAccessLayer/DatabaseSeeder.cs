using Infrastructure.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using DataAccessLayer.DTO;
using Bogus.DataSets;

namespace DataAccessLayer
{
    public class DatabaseSeeder(ChatRepository chatRepository, UserRepository userRepository, int userCount, int messageCount, int chatCount)
    {
        private ChatRepository _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
        private UserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private int _userCount = userCount;
        private int _messageCount = messageCount;
        private int _chatCount = chatCount;



        public void SeedDatabase()
        {
            _chatRepository.ClearDatabase();
            List<UserDTO> users = GetRandomUsers(_userCount).ToList();
            List<ChatDTO> chats = GetRandomChats(_chatCount, users).ToList();
            GenerateRandomMessages(chats, _messageCount);
        }

        public IEnumerable<ChatDTO> GetRandomChats(int count, List<UserDTO> users)
        {
            var faker = new Faker();
            for (int i = 0; i <= count; i++)
            {
                var members = faker.Random.ListItems(users, faker.Random.Int(2, users.Count));
                foreach (var member in members)
                {
                    member.IsAdmin = faker.Random.Bool(0.1f);
                }
                var owner = faker.PickRandom(users);
                owner.IsAdmin = true;
                string name = "";
                bool IsGroupChat = false;
                if(faker.Random.Bool(0.3f))
                {
                    members = faker.Random.ListItems(members, 2);
                    name = members[0].FirstName + "&" + members[1].FirstName;
                    IsGroupChat = false;
                } else
                {
                    name = faker.Commerce.ProductName();
                    IsGroupChat = true;
                }

                yield return _chatRepository.CreateChat(new ChatDTO(name, members.ToList()) { IsGroupChat = IsGroupChat }, owner);
            }
        }

        public IEnumerable<UserDTO> GetRandomUsers(int count)
        {
            var faker = new Faker();
            for(int i = 0; i <= count; i++)
            {
                string firstname = faker.Name.FirstName();
                string lastName = faker.Name.LastName();
                string fullName = firstname + " " + lastName;

                yield return _userRepository.CreateUser(new UserDTO(faker.Internet.Email(fullName), null, null)
                {
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    Gender = faker.PickRandom(new[] { "Male", "Female", "Other" }),
                    Street = faker.Address.StreetAddress(),
                    City = faker.Address.City(),
                    HouseNumber = faker.Random.Int(1, 999).ToString(),
                    BirthDate = faker.Date.Past(50, DateTime.Today.AddYears(-60)), 
                    SearchRadius = faker.Random.Int(0, 50), 
                    Zipcode = faker.Address.ZipCode(),
                    Initials = firstname[0].ToString() + lastName[1].ToString(),
                    Country = faker.Address.CountryCode()
                }, "fontys123");
            }
        }

        public void GenerateRandomMessages(List<ChatDTO> chats, int maxCount)
        {
            var faker = new Faker();
            foreach(ChatDTO chat in chats){
                for (int i = faker.Random.Int(1, maxCount - 1); i < maxCount; i++)
                {
                    string randomMessage = faker.Rant.Review(faker.PickRandom(chat.Users).FirstName);
                    _chatRepository.CreateMessage(chat.Id, new MessageDTO(randomMessage, faker.PickRandom(chat.Users), faker.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now)));
                }
            }
        }
    }
}