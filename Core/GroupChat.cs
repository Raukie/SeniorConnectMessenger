using DataAccessLayer.DTO;

namespace Core
{
    public class GroupChat(string name, string hash, int id, MessageDTO lastReadMessage) : Chat(name, hash, id, lastReadMessage)
    {
        
    }
}
