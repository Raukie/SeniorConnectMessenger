﻿using core.Helpers;
using Core.Models.DTO;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorConnectMessengerWeb.Helpers;
using SeniorConnectMessengerWeb.Models.DTO;
using System.Runtime.Serialization.Json;

namespace SeniorConnectMessengerWeb.Controllers
{
    [Authorize]
    public class ChatController(UserService userService, ChatService chatService, IChatStorage chatRepository): Controller
    {
        private UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private ChatService _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));

        /// <summary>
        /// Only used to pass to the domain classes, nothing else!
        /// </summary>
        private IChatStorage _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));

        public IResult GetUserChats()
        {
            int userId = _userService.GetCurrentUserId(HttpContext);
            var chats = chatService.GetAllChatsDataUserIsIn(userId);

            return Results.Ok(chats);
        }

        public IResult GetChatContent(int chatId)
        {
            int userId = _userService.GetCurrentUserId(HttpContext);
            var chat = _chatService.GetChatData(chatId, userId, true);
            return Results.Ok(chat);
        }
        [HttpPost()]
        public IResult PollForUpdates(List<ChatPollDTO> chatsToPoll)
        {
			int userId = _userService.GetCurrentUserId(HttpContext);
			var chatsToUpdate = _chatService.FetchChatUpdates(userId, chatsToPoll);
			return Results.Ok(chatsToUpdate);
        }

        [HttpPost()]
        public IResult RemoveUserFromChat(int chatId, int userId)
        {
            int loggedInUserId = _userService.GetCurrentUserId(HttpContext);
            var groupChat = _chatService.GetGroupChat(chatId, userId);

            var loggedInUser = _userService.GetUser(loggedInUserId);
            var user = _userService.GetUser(userId);

            // no error message needed, could only be malicious
            if (loggedInUser == null || user == null)
            {
                return Results.Problem();
            }

            groupChat.RemoveUserFromChat(_chatRepository, user, loggedInUser);
            return Results.Ok();
        }

        [HttpPost()]
        public IResult MakeUserAdmin(int chatId, int userId)
        {
            int loggedInUserId = _userService.GetCurrentUserId(HttpContext);
            var groupChat = _chatService.GetGroupChat(chatId, userId);

            var loggedInUser = _userService.GetUser(loggedInUserId);
            var user = _userService.GetUser(userId);

            // no error message needed, could only be malicious
            if (loggedInUser == null || user == null)
            {
                return Results.Problem();
            }

            groupChat.MakeUserAdmin(_chatRepository, user, loggedInUserId);
            return Results.Ok();
        }
    }
}
