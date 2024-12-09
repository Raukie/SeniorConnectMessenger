using Core;
using Core.Models.DTO;
using Core.ObjectMapper;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using System.Linq;
using System.Net.Http;

namespace core.Helpers
{
    public class ChatService(IChatStorage chatRepository)
    {
        private IChatStorage _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
        public List<Chat> GetAllChatsUserIsIn(int userId)
        {
            var chats = chatRepository.GetChatsUserIsIn(userId);

            return chats.Select(chat => new Chat(chat.Name, chat.Hash, chat.Id, chat.LastReadMessage, chat.AmountOfUnreadMessages!.Value)).ToList();
        }

		public List<ChatDTO> GetAllChatsDataUserIsIn(int userId)
		{
			return _chatRepository.GetChatsUserIsIn(userId);
		}

        public ChatDTO GetChatData(int chatId, int userId, bool updateLastReadMessage)
        {
            return _chatRepository.GetChat(chatId, userId, updateLastReadMessage);
        }

        public Chat GetChat(int chatId, int userId, bool updateLastReadMessage)
        {
            return _chatRepository.GetChat(chatId, userId, updateLastReadMessage).ToDomain();
        }

        public GroupChat GetGroupChat(int chatId, int userId)
		{
<<<<<<< Updated upstream
			return (GroupChat)_chatRepository.GetChat(chatId, userId, false).ToDomain();
=======
			return _chatRepository.GetChat(chatId, userId, false).ToGroupChatDomain();
>>>>>>> Stashed changes
		}

        public List<ChatUpdateDTO> FetchChatUpdates(int userId, List<ChatPollDTO> chatsToPoll)
        {
			var chats = GetAllChatsUserIsIn(userId);
			List<ChatUpdateDTO> chatUpdatesDTO = new List<ChatUpdateDTO>();

			foreach (var chatToPoll in chatsToPoll)
			{
				var chat = chats.FirstOrDefault(cht => cht.Id == chatToPoll.Id);
				//chat has been removed
				if (chat == null)
				{
					chatUpdatesDTO.Add(new ChatUpdateDTO() { Removed = true, Id = chatToPoll.Id });
					continue;
				}
				ChatUpdateDTO chatUpdate = new() { Id = chat.Id };
				chatUpdate.Hash = chatToPoll.Hash;

				// if hash is different the name is altered or the users. Update name here
				if (chat.ShouldUpdateUI(chatToPoll.Hash))
				{
					chatUpdate.Hash = chat.Hash;
					chatUpdate.Name = chat.Name;
				}

				if (chatToPoll.IsOpen)
				{
					chatUpdate.messages = chat.GetUnreadMessagesInChat(_chatRepository, userId);
					// no reason to update if there are no new messages
					if (!chatUpdate.messages.Any())
					{
						continue;
					}
				}
				else if (chat.GetLastMessage().Id != chatToPoll.LastFetchedMessageId)
				{
					chatUpdate.messages.Add(chat.GetLastMessage());
					chatUpdate.AmountOfUnreadMessages = chat.UnreadMessagesCount;
				}
				else
				{
					continue;
				}
				chatUpdatesDTO.Add(chatUpdate);
			}

			return chatUpdatesDTO;
		}
	}
}
