using DataAccessExtensions;
using DataAccessExtensions.Extensions;
using DataAccessLayer.DTO;
using DataAccessLayer.Extensions;
using Microsoft.Data.SqlClient;

namespace Infrastructure.DataAccessLayer
{
    /// <summary>
    /// The chat repository class should be used for fetching and alterting
    /// data in the database.
    /// </summary>
    public class UserRepository(string connectionString) : Repository(connectionString), IUserStorage
    {
        public string GetUserPasswordHash(UserDTO user)
        {
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"
                    SELECT Password FROM [Users] WHERE ID = @UserId
                ");

                command.Parameters.AddWithValue("@UserId", user.Id);

                return (string)command.ExecuteScalar();
            }
        }

        public UserDTO? GetUserByUsername(string userName)
        {
            UserDTO? userDto = null;

            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"SELECT U.ID, U.Username, U.Password,
                UP.FirstName, UP.LastName, UP.Gender, UP.Street, UP.City, UP.HouseNumber, UP.BirthDate, 
                UP.SearchRadius, UP.Zipcode, UP.Initials, UP.Country
                FROM [Users] U
                INNER JOIN [UserProfile] UP ON U.ID = UP.UserID
                WHERE U.Username = @Username");

                command.Parameters.AddWithValue("@Username", userName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userDto = new UserDTO(reader["Username"].ToString(), (int)reader["ID"])
                        {
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Street = reader["Street"].ToString(),
                            City = reader["City"].ToString(),
                            HouseNumber = reader["HouseNumber"].ToString(),
                            BirthDate = (DateTime)reader["BirthDate"],
                            SearchRadius = (int)reader["SearchRadius"],
                            Zipcode = reader["Zipcode"].ToString(),
                            Initials = reader["Initials"].ToString(),
                            Country = reader["Country"].ToString()
                        };
                    }
                }
            }
            return userDto;
        }

        public UserDTO? GetUserById(int id)
        {
            UserDTO? userDto = null;

            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"SELECT U.ID, U.Username, U.Password,
                UP.FirstName, UP.LastName, UP.Gender, UP.Street, UP.City, UP.HouseNumber, UP.BirthDate, 
                UP.SearchRadius, UP.Zipcode, UP.Initials, UP.Country
                FROM [Users] U
                INNER JOIN [UserProfile] UP ON U.ID = UP.UserID
                WHERE U.ID = @UserId");

                command.Parameters.AddWithValue("@UserId", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userDto = new UserDTO(reader["Username"].ToString(), (int)reader["ID"])
                        {
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Street = reader["Street"].ToString(),
                            City = reader["City"].ToString(),
                            HouseNumber = reader["HouseNumber"].ToString(),
                            BirthDate = (DateTime)reader["BirthDate"],
                            SearchRadius = (int)reader["SearchRadius"],
                            Zipcode = reader["Zipcode"].ToString(),
                            Initials = reader["Initials"].ToString(),
                            Country = reader["Country"].ToString()
                        };
                    }
                }
            }
            return userDto;
        }
        /// <summary>
        /// Password is hashed here
        /// </summary>
        /// <param name="userDTO"></param>
        /// <param name="plainPassword">Is going to be hashed</param>
        public UserDTO CreateUser(UserDTO userDTO, string plainPassword)
        {
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"INSERT INTO [Users]
                    ([Username], [Password]) output INSERTED.ID VALUES (@Username, @Password)
                ");

                command.Parameters.AddWithValue("@Username", userDTO.Username);
                command.Parameters.AddWithValue("@Password", DataHelper.HashPassword(plainPassword));
                userDTO.Id = (int)command.ExecuteScalar();

                command = transaction.CreateCommand(@"INSERT INTO [UserProfile]
                    ([UserID], [FirstName], [LastName], [Gender], [Street], [City], [HouseNumber], [BirthDate], [SearchRadius], [Zipcode], [Initials], [Country])
                    VALUES (@UserId, @FirstName, @LastName, @Gender, @Street, @City, @HouseNumber, @BirthDate, @SearchRadius, @Zipcode, @Initials, @Country)
                ");

                command.Parameters.AddWithValue("@UserId", userDTO.Id);
                command.Parameters.AddWithValue("@FirstName", userDTO.FirstName);
                command.Parameters.AddWithValue("@LastName", userDTO.LastName);
                command.Parameters.AddWithValue("@Gender", userDTO.Gender);
                command.Parameters.AddWithValue("@Street", userDTO.Street);
                command.Parameters.AddWithValue("@City", userDTO.City);
                command.Parameters.AddWithValue("@HouseNumber", userDTO.HouseNumber);
                command.Parameters.AddWithValue("@BirthDate", userDTO.BirthDate);
                command.Parameters.AddWithValue("@SearchRadius", userDTO.SearchRadius);
                command.Parameters.AddWithValue("@Zipcode", userDTO.Zipcode);
                command.Parameters.AddWithValue("@Initials", userDTO.Initials);
                command.Parameters.AddWithValue("@Country", userDTO.Country);
                command.ExecuteNonQuery();
            }
            return userDTO;
        }
    }
}