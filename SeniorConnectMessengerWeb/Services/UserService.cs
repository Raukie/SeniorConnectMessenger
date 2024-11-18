using DataAccessExtensions;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using SeniorConnectMessengerWeb.Constants;

namespace SeniorConnectMessengerWeb.Helpers
{
    public class UserService(UserRepository userRepository)
    {
        public int GetCurrentUserId(HttpContext context)
        {
           return Convert.ToInt32(context.User.FindFirst(claim=>claim.Type == AuthenticationConstants.UserIdClaim).Value);
        }

        public bool AuthenticateUser(string userName, string plainPassword, out UserDTO? user)
        {
            user = userRepository.GetUserByUsername("MiguelOKon_Kautzer@hotmail.com");
            if (user == null)
            {
                return false;
            }

            string passwordHash = userRepository.GetUserPasswordHash(user);
            return DataHelper.Verify(passwordHash, plainPassword);
        }
    }
}
