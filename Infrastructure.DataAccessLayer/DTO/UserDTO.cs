using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class UserDTO
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isAdmin"> It is a unmapped property used for the groupChats</param>
        /// <param name="ID"> It is a nullable property if null the user is not yet in the database</param>
        public UserDTO(string username, int? id = null, bool? isAdmin = null)
        {
            Username = username;
            IsAdmin = isAdmin ;
            Id = id;
        }
        public string Username { get; set; }
        public string? Initials { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? City { get; set; }
        public string? Zipcode { get; set; }
        public string? Country { get; set; }
        public int? SearchRadius { get; set; }
        public bool? IsAdmin { get; set; }
        public int? Id { get; set; }
    }
}