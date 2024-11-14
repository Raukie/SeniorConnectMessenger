using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;

namespace SeniorConnectMessengerWeb.Helpers
{
    public class UserService(UserRepository userRepository)
    {
        public int GetCurrentUserId(HttpContext context)
        {
            var user = userRepository.GetUserByUsername("LayneErnser28@yahoo.com");
            return user?.Id ?? 0;
        }
    }
}
