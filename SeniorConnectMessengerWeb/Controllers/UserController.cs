using core.Helpers;
using Core.Models.DTO;
using Core.Services;
using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorConnectMessengerWeb.Helpers;
using SeniorConnectMessengerWeb.Models.DTO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace SeniorConnectMessengerWeb.Controllers
{
    [Authorize]
    public class UserController(UserService userService, ChatService chatService, IChatStorage chatRepository, ChatUpdateService chatUpdateService) : Controller
    {
        private UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private ChatService _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
        private ChatUpdateService _chatUpdateService = chatUpdateService ?? throw new ArgumentNullException(nameof(chatUpdateService));

        /// <summary>
        /// Only used to pass to the domain classes, nothing else!
        /// </summary>
        
        public IResult FindUsers(string query)
        {
            return Results.Ok(_userService.FindUsers(query));
        }
    }
}
