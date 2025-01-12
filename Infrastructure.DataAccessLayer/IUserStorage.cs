using DataAccessLayer.DTO;

namespace Infrastructure.DataAccessLayer
{
    public interface IUserStorage
    {
        UserDTO CreateUser(UserDTO userDTO, string plainPassword);
        UserDTO? GetUserById(int id);
        UserDTO? GetUserByUsername(string userName);
        List<UserDTO> FindUser(string searchQuery);
        string GetUserPasswordHash(UserDTO user);
    }
}