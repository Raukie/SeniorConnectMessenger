using Bogus;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class BetterDatabaseSeeder
    {
        private readonly ChatRepository _chatRepository;
        private readonly UserRepository _userRepository;

        // Aantal extra random gebruikers die geen berichten versturen:
        private readonly int _extraUserCount;

        public BetterDatabaseSeeder(
            ChatRepository chatRepository,
            UserRepository userRepository,
            int extraUserCount = 10)
        {
            _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _extraUserCount = extraUserCount;
        }

        public void SeedDatabase()
        {
            // Verwijder oude data
            _chatRepository.ClearDatabase();

            // 1. Maak de vaste groepsleden aan
            var groupUsers = CreateFixedGroupUsers();

            // 2. Maak extra willekeurige gebruikers aan (die geen berichten sturen)
            var randomUsers = CreateExtraRandomUsers(_extraUserCount);

            // Voeg deze lijst samen; (optioneel) je kunt ze ook apart houden
            var allUsers = groupUsers.Concat(randomUsers).ToList();

            // 3. Maak 1 of 2 chats aan waar ALLEEN de groepsleden in zitten
            var groupChats = CreateGroupChats(groupUsers);

            // 4. Voeg voorbeeldberichten toe aan deze chats (met reële data)
            AddSampleMessages(groupChats);

            // Klaar!
        }

        /// <summary>
        /// Maakt vaste groepsleden aan met hun specifieke data.
        /// </summary>
        private List<UserDTO> CreateFixedGroupUsers()
        {
            var groupUsers = new List<UserDTO>();
            var faker = new Faker("nl"); // "nl" voor Nederlandse data, indien gewenst

            // Daan Steuten
            groupUsers.Add(_userRepository.CreateUser(new UserDTO("d.steuten@student.fontys.nl", null, null)
            {
                FirstName = "Daan",
                LastName = "Steuten",
                BirthDate = new DateTime(2005, 1, 13),        // onbekend
                Gender = "Male",         // voorbeeld
                City = "Venlo",          // kies iets
                Initials = "D.S",
                Street = faker.Address.StreetAddress(),
                HouseNumber = faker.Random.Int(1, 250).ToString(),
                Zipcode = faker.Address.ZipCode(),
                Country = faker.Address.CountryCode(),
                SearchRadius = faker.Random.Int(1, 200)
            }, "fontys123"));

            // Ryan Evertz
            groupUsers.Add(_userRepository.CreateUser(new UserDTO("r.evertz@student.fontys.nl", null, null)
            {
                FirstName = "Ryan",
                LastName = "Evertz",
                BirthDate = new DateTime(2005, 1, 13), // 13.01.2005
                Gender = "Male",
                City = "Eindhoven",
                Initials = "R.E",
                Street = faker.Address.StreetAddress(),
                HouseNumber = faker.Random.Int(1, 250).ToString(),
                Zipcode = faker.Address.ZipCode(),
                Country = faker.Address.CountryCode(),
                SearchRadius = faker.Random.Int(1, 200)
            }, "fontys123"));

            // Damian Luijkx
            groupUsers.Add(_userRepository.CreateUser(new UserDTO("d.luijkx@student.fontys.nl", null, null)
            {
                BirthDate = new DateTime(2005, 1, 13),
                FirstName = "Damian",
                LastName = "Luijkx",
                Gender = "Male",
                City = "Tilburg",
                Initials = "D.L",
                Street = faker.Address.StreetAddress(),
                HouseNumber = faker.Random.Int(1, 250).ToString(),
                Zipcode = faker.Address.ZipCode(),
                Country = faker.Address.CountryCode(),
                SearchRadius = faker.Random.Int(1, 200)
            }, "fontys123"));

            // Tom van Tilburg
            groupUsers.Add(_userRepository.CreateUser(new UserDTO("t.vantilburg@student.fontys.nl", null, null)
            {
                BirthDate = new DateTime(2005, 1, 13),
                FirstName = "Tom",
                LastName = "van Tilburg",
                Gender = "Male",
                City = "Breda",
                Initials = "T.V.T",
                Street = faker.Address.StreetAddress(),
                HouseNumber = faker.Random.Int(1, 250).ToString(),
                Zipcode = faker.Address.ZipCode(),
                Country = faker.Address.CountryCode(),
                SearchRadius = faker.Random.Int(1, 200)
            }, "fontys123"));

            // Gijs Dijk
            groupUsers.Add(_userRepository.CreateUser(new UserDTO("g.dijk@student.fontys.nl", null, null)
            {BirthDate = new DateTime(2005, 1, 13),
                FirstName = "Gijs",
                LastName = "Dijk",
                Gender = "Male",
                City = "Den Bosch",
                Initials = "G.D",
                Street = faker.Address.StreetAddress(),
                HouseNumber = faker.Random.Int(1, 250).ToString(),
                Zipcode = faker.Address.ZipCode(),
                Country = faker.Address.CountryCode(),
                SearchRadius = faker.Random.Int(1, 200)
            }, "fontys123"));

            // René van de Berg
            groupUsers.Add(_userRepository.CreateUser(new UserDTO("r.vandenberg@student.fontys.nl", null, null)
            {
                BirthDate = new DateTime(2005, 1, 13),
                FirstName = "René",
                LastName = "van de Berg",
                Gender = "Male",
                City = "Helmond",
                Initials = "R.V.D.B",
                Street = faker.Address.StreetAddress(),
                HouseNumber = faker.Random.Int(1, 250).ToString(),
                Zipcode = faker.Address.ZipCode(),
                Country = faker.Address.CountryCode(),
                SearchRadius = faker.Random.Int(1, 200)

            }, "fontys123"));

            return groupUsers;
        }

        /// <summary>
        /// Maakt extra willekeurige gebruikers aan, maar laat ze geen berichten sturen.
        /// </summary>
        private List<UserDTO> CreateExtraRandomUsers(int count)
        {
            var faker = new Faker("nl"); // "nl" voor Nederlandse data, indien gewenst
            var randomUsers = new List<UserDTO>();

            for (int i = 0; i < count; i++)
            {
                string fname = faker.Name.FirstName();
                string lname = faker.Name.LastName();

                var user = new UserDTO(faker.Internet.Email($"{fname} {lname}"), null, null)
                {
                    FirstName = fname,
                    LastName = lname,
                    Gender = faker.PickRandom(new[] { "Male", "Female", "Other" }),
                    City = faker.Address.City(),
                    BirthDate = faker.Date.Past(50, DateTime.Today.AddYears(-18)), // Tussen 18 en 68 jaar
                    Initials = $"{fname[0]}.{lname[0]}",
                    Street = faker.Address.StreetAddress(),
                    HouseNumber = faker.Random.Int(1, 250).ToString(),
                    Zipcode = faker.Address.ZipCode(),
                    Country = faker.Address.CountryCode(),
                    SearchRadius = faker.Random.Int(1, 200)
                };

                randomUsers.Add(_userRepository.CreateUser(user, "fontys123"));
            }

            return randomUsers;
        }

        private List<ChatDTO> CreateGroupChats(List<UserDTO> groupUsers)
        {
            var chats = new List<ChatDTO>();

            // Voorbeeld van een aantal "vaste" subsets voor de 8 chats:
            // Je kunt hier ook Bogus gebruiken om subsets te randomizen, 
            // of zelf andere subsets definiëren.
            var chatDefinitions = new List<(string Name, List<UserDTO> Users, int MonthsAgo)>
    {
        // Chat 1: Iedereen
        ("SeniorConnect Overleg", groupUsers, 2),

        // Chat 2: Eerste 3
        ("Infra & Dev Discussie", groupUsers.Take(3).ToList(), 1),

        // Chat 3: Drie andere
        ("Docker Brainstorm", groupUsers.Skip(1).Take(3).ToList(), 3),

        // Chat 4: Iets grotere groep
        ("Scope Overleg", groupUsers.Skip(2).Take(4).ToList(), 4),

        // Chat 5: Nog een paar
        ("Backend Focus", groupUsers.Take(5).ToList(), 2),

        // Chat 6
        ("IIS Setup Chat", groupUsers.Skip(1).Take(5).ToList(), 1),

        // Chat 7
        ("Kerstplanning", groupUsers, 1),

        // Chat 8
        ("Afstudeer Overleg", groupUsers.Skip(3).Take(3).ToList(), 1)
    };

            // Voor elk gedefinieerd "chat-koppel" maken we de ChatDTO en slaan we hem op
            foreach (var (name, users, monthsAgo) in chatDefinitions)
            {
                // Maak de chat aan
                var chatDTO = new ChatDTO(name, users) { IsGroupChat = true };

                // Stel Owner in (eerste user in de subset) en zet die isAdmin op true
                var owner = users.FirstOrDefault();
                if (owner != null)
                {
                    owner.IsAdmin = true;
                }

                // De rest isAdmin = false
                foreach (var user in users.Where(u => u != owner))
                {
                    user.IsAdmin = false;
                }

                // Aanmaken in de database
                var createdChat = _chatRepository.CreateChat(
                    chatDTO,
                    owner,
                    DateTime.Now.AddMonths(-monthsAgo)
                );

                chats.Add(createdChat);
            }

            return chats;
        }

        /// <summary>
        /// Voegt voorbeeldberichten toe in ALLE aangemaakte chats
        /// (Niet alleen in de eerste twee).
        /// </summary>
        private void AddSampleMessages(List<ChatDTO> chats)
        {
            if (!chats.Any()) return;

            // Extra voorbeeldberichten met wat fictieve context
            var sampleMessages = new List<(string Content, string SenderFirstName, DateTime SentAt)>
    {
        ("Beste heren, allereerst: bekijk goed en corrigeer waar nodig...", "Tom", DateTime.Parse("2024-12-18 22:07")),
        ("Ik denk dat dit wel erg veel is om voor 19-1-2025 te realiseren...", "Gijs", DateTime.Parse("2024-12-19 10:27")),
        ("Ja dat denk ik ook Gijs, heb zelf ook geen kennis van Docker...", "Ryan", DateTime.Parse("2024-12-19 10:33")),
        ("Daan en ik kunnen een infrastructuur opzetten met een VM...", "Damian", DateTime.Parse("2024-12-19 13:29")),
        ("Fijne kerst allemaal!", "Tom", DateTime.Parse("2024-12-20 11:01")),
        ("Ik ben er vanavond niet, moet nog iets met werk doen tot 23:00...", "Damian", DateTime.Parse("2024-12-20 13:09")),
        ("Geen probleem, we pakken het na de feestdagen weer op!", "Tom", DateTime.Parse("2024-12-20 14:22")),
        ("Kan iemand Azure Bastion voor me klaarzetten?", "Ryan", DateTime.Parse("2024-12-20 14:30")),
        ("Dat doe ik wel, Ryan. Stuur me even de details via mail.", "Damian", DateTime.Parse("2024-12-20 14:45")),
        ("Ziet er allemaal goed uit, ik zal de rest van de planning vandaag afronden!", "Daan", DateTime.Parse("2024-12-20 15:10")),
        ("We moeten ook nog even checken wie de user stories gaat uitwerken.", "Gijs", DateTime.Parse("2024-12-21 09:05")),
        ("Ik kan er vanavond naar kijken, even zien wat haalbaar is.", "Tom", DateTime.Parse("2024-12-21 21:07")),
        ("Thanks Tom. Ik heb alvast wat kernpunten op de backlog gezet.", "Ryan", DateTime.Parse("2024-12-22 11:44")),
        ("Top, dan bespreken we het na de vakantie verder.", "Daan", DateTime.Parse("2024-12-22 12:00")),
        ("Heeft iemand al gekeken naar CI/CD met Azure DevOps?", "René", DateTime.Parse("2024-12-22 12:10")),
        ("Nog niet, maar ik kan daar morgen even naar kijken.", "Damian", DateTime.Parse("2024-12-22 12:15")),
        ("Lijkt me goed. Houd ons op de hoogte, dan kan ik de documentatie bijwerken.", "Tom", DateTime.Parse("2024-12-22 12:20")),
        ("Gaan we ook Docker gebruiken voor de testomgeving of eerst alleen productie?", "Ryan", DateTime.Parse("2024-12-22 14:30")),
        ("Ik stel voor om het eerst lokaal te testen en dan pas door te trekken naar productie.", "Gijs", DateTime.Parse("2024-12-22 14:45")),
    };

            // Voor elke chat in onze lijst:
            foreach (var chat in chats)
            {
                // Haal de gebruikers erbij, zodat we de afzender kunnen matchen op voornaam
                var chatUsers = chat.Users;

                // Voeg elk bericht toe, mits de afzender in deze chat zit
                foreach (var (content, senderName, sentAt) in sampleMessages)
                {
                    var sender = chatUsers.FirstOrDefault(u =>
                        u.FirstName?.Equals(senderName, StringComparison.OrdinalIgnoreCase) == true);

                    if (sender != null)
                    {
                        _chatRepository.CreateMessage(
                            chat.Id,
                            new MessageDTO(content, sender, sentAt)
                        );
                    }
                }
            }
        }


    }
}
