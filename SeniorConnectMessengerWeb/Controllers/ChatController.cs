using core.Helpers;
using Core.Models.DTO;
using Core.Services;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeniorConnectMessengerWeb.Helpers;
using SeniorConnectMessengerWeb.Models.DTO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace SeniorConnectMessengerWeb.Controllers
{
    [Authorize]
    public class ChatController(UserService userService, ChatService chatService, IChatStorage chatRepository,
        ChatUpdateService chatUpdateService): Controller
    {
        private UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private ChatService _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        private ChatUpdateService _chatUpdateService = chatUpdateService ?? throw new ArgumentNullException(nameof(chatUpdateService));

        /// <summary>
        /// Only used to pass to the domain classes, nothing else!
        /// </summary>
        private IChatStorage _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));

        public IResult AddUser(int userId, int chatId)
        {
            var groupChat = _chatService.GetGroupChat(chatId, userId);
            int loggedInUserId = _userService.GetCurrentUserId(HttpContext);

            var loggedInUser = _userService.GetUser(loggedInUserId);
            var user = _userService.GetUser(userId);

            if(groupChat.AddUser(_chatRepository, user!, loggedInUser!))
            {
                return Results.Ok();
            } else
            {
                return Results.Problem();
            }
        }

        public IResult CreateChat(string chatName, List<int> userIds)
        {
            // there are already front end checks for these
            if(chatName.Length < 5 || chatName.Length > 35 || userIds.Count < 2)
            {
                return Results.Problem();
            }

            int userId = _userService.GetCurrentUserId(HttpContext);
            var user = _userService.GetUser(userId);
            
            if (_chatService.CreateChat(WebUtility.HtmlEncode(chatName), user!, userIds))
            {
                return Results.Ok();
            } else
            {
                return Results.Problem();
            }
        }

        public IResult GetUserChats()
        {
            int userId = _userService.GetCurrentUserId(HttpContext);
            var chats = chatService.GetAllChatsDataUserIsIn(userId);

            return Results.Ok(chats);
        }

        public IResult SendMessage(int chatId, string messageContent)
        {
            int userId = _userService.GetCurrentUserId(HttpContext);
            var chat = _chatService.GetChat(chatId, userId, false);
            var user = _userService.GetUser(userId);

            if(user == null || chat.SendMessage(chatRepository, user, WebUtility.HtmlEncode(messageContent)))
            {
                return Results.Ok();
            } else
            {
                return Results.Problem("Kon bericht niet versturen");
            }
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
			var chatsToUpdate = _chatUpdateService.FetchChatUpdates(userId, chatsToPoll);
			return Results.Ok(chatsToUpdate);
        }

        [HttpPost()]
        public IResult LeaveChat(int chatId)
        {
            int userId = _userService.GetCurrentUserId(HttpContext);
            var user = _userService.GetUser(userId);

            var chat = _chatService.GetChat(chatId, userId, false);
            chat.UserLeaveChat(chatRepository, user);

            return Results.Ok();
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
