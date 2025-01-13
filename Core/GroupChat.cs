using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;

namespace Core
{
    public class GroupChat(string name, string hash, int id, MessageDTO? lastReadMessage, int? unreadMessagesCount) : Chat(name, hash, id, lastReadMessage, unreadMessagesCount)
    {
        public bool RemoveUserFromChat(IChatStorage chatRepository, UserDTO user, UserDTO userActor)
        {
            if (!IsUserAdmin(chatRepository, userActor.Id!.Value))
            {
                return false;
            }

            chatRepository.CreateMessage(_id, 
                new MessageDTO($"{user.FirstName} {user.LastName} was verwijderd door {userActor.FirstName} {userActor.LastName}")
            );

            return chatRepository.RemoveUserFromChat(_id, user.Id!.Value);
        }

        public bool AddUser(IChatStorage chatRepository, UserDTO user, UserDTO userActor)
        {
            if (!IsUserAdmin(chatRepository, userActor.Id!.Value))
            {
                return false;
            }

            if(!chatRepository.AddUserToChat(user, this.Id))
            {
                return false;
            }

            chatRepository.CreateMessage(_id,
                new MessageDTO($"{user.FirstName} {user.LastName} heeft {userActor.FirstName} {userActor.LastName} toegevoegd")
            );

            return true;
        }
        public bool MakeUserAdmin(IChatStorage chatRepository, UserDTO user, int userIdActor)
        {
            if (!IsUserAdmin(chatRepository, userIdActor))
            {
                return false;
            }

            chatRepository.CreateMessage(_id, new MessageDTO($"{user.FirstName} {user.LastName} is nu beheerder"));

            return chatRepository.MakeUserAdmin(_id, user.Id!.Value);
        }

        public bool IsUserAdmin(IChatStorage chatRepository, int userId)
        {
            return chatRepository.IsUserAdmin(_id, userId);
        }
    }
}
