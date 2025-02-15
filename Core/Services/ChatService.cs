﻿using Azure.Messaging;
using Core;
using Core.Models.DTO;
using Core.ObjectMapper;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace core.Helpers
{
    public class ChatService(IChatStorage chatRepository, IUserStorage userRepository)
    {
        private IChatStorage _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
        private IUserStorage _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        public List<Chat> GetAllChatsUserIsIn(int userId)
        {
            var chats = chatRepository.GetChatsUserIsIn(userId);

            return chats.Select(chat => new Chat(chat.Name, chat.Hash, chat.Id, chat.LastReadMessage, chat.AmountOfUnreadMessages!.Value)).ToList();
        }

		public bool CreateChat(string chatName, UserDTO createdBy, List<int> userIds)
		{
			var users = userIds.Select(userId => _userRepository.GetUserById(userId)).ToList();

			if(!users.Any(user=>user.Id == createdBy.Id))
			{
				createdBy.IsAdmin = true;
				users.Add(createdBy);
			}

			foreach(var user in users){
				user.IsAdmin = (user.Id == createdBy.Id);
			}

			_chatRepository.CreateChat(new ChatDTO()
			{
				Users = users,
				IsGroupChat = true,
				Name = chatName
			}, createdBy);

			return true;
		}

		public List<ChatDTO> GetAllChatsDataUserIsIn(int userId)
		{
			return _chatRepository.GetChatsUserIsIn(userId).OrderByDescending(chat=>chat.LastReadMessage.SendAt).ToList();
		}

        public ChatDTO GetChatData(int chatId, int userId, bool updateLastReadMessage)
        {
            return _chatRepository.GetChat(chatId, userId, updateLastReadMessage);
        }

        public Chat GetChat(int chatId, int userId, bool updateLastReadMessage)
        {
            return _chatRepository.GetChat(chatId, userId, updateLastReadMessage).ToChatDomain();
        }

        public GroupChat GetGroupChat(int chatId, int userId)
		{
			return _chatRepository.GetChat(chatId, userId, false).ToGroupChatDomain();
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
