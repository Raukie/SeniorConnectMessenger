using DataAccessExtensions;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using SeniorConnectMessengerWeb.Constants;
using SeniorConnectMessengerWeb.Models.DTO.User;

namespace SeniorConnectMessengerWeb.Helpers
{
    public class UserService(IUserStorage userRepository)
    {
        public List<FoundUser> FindUsers(string query)
        {
            return userRepository.FindUser(query)
                .Select(user=> 
                    new FoundUser() { FirstName = user.FirstName, LastName = user.LastName, ID = user.Id.Value }
                ).ToList();
        }
        public int GetCurrentUserId(HttpContext context)
        {
           return Convert.ToInt32(context.User.FindFirst(claim => claim.Type == AuthenticationConstants.UserIdClaim).Value);
        }

        public UserDTO? GetUser(int userId)
        {
            return userRepository.GetUserById(userId);
        }

        public bool AuthenticateUser(string userName, string plainPassword, out UserDTO? user)
        {
            user = userRepository.GetUserByUsername(userName);
            if (user == null)
            {
                return false;
            }

            string passwordHash = userRepository.GetUserPasswordHash(user);
            return DataHelper.Verify(passwordHash, plainPassword);
        }
    }
}
