using DataAccessExtensions;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.MockDataAccess
{
    internal class MockUserStorage : IUserStorage
    {
        internal List<UserDTO> Users { get; set; } = new();

        // Used for testing password hashing and verification, not mapped to DTO for security reasons
        internal Dictionary<int, string> UserPasswords { get; set; } = new(); 

        public UserDTO CreateUser(UserDTO userDTO, string plainPassword)
        {
            Users.Add(userDTO);
            var user = Users.Last();
            user.Id = Users.Count;
            UserPasswords.Add(user.Id.Value, DataHelper.HashPassword(plainPassword));
            return user;
        }

        public UserDTO? GetUserById(int id)
        {
            return Users.FirstOrDefault(user => user.Id == id);
        }

        public UserDTO? GetUserByUsername(string userName)
        {
            return Users.FirstOrDefault(user=>user.Username == userName);
        }

        public string GetUserPasswordHash(UserDTO user)
        {
            return UserPasswords[user.Id!.Value];
        }
    }
}
