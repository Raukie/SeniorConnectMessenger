using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;

namespace SeniorConnectMessengerWeb.Helpers
{
    public class UserService(UserRepository userRepository)
    {
        public int GetCurrentUserId(HttpContext context)
        {
            var user = userRepository.GetUserByUsername("MiguelOKon_Kautzer@hotmail.com");
            return user?.Id ?? 0;
        }
    }
}
