using DataAccessLayer.DTO;

namespace Infrastructure.DataAccessLayer
{
    public interface IUserStorage
    {
        UserDTO CreateUser(UserDTO userDTO, string plainPassword);
        UserDTO? GetUserById(int id);
        UserDTO? GetUserByUsername(string userName);
        string GetUserPasswordHash(UserDTO user);
    }
}