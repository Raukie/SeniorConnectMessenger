using DataAccessLayer.DTO;
using Infrastructure.DataAccessLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SeniorConnectMessengerWeb.Constants;
using SeniorConnectMessengerWeb.Helpers;
using SeniorConnectMessengerWeb.Models.DTO.Auth;
using System.Security.Claims;

namespace SeniorConnectMessengerWeb.Controllers
{
    public class AuthController(UserService userService) : Controller
    {
        private UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Endpoint to authenticate user.
        /// Not neccecary to return error message for invalid model since it is also handled on the front end
        /// </summary>
        /// <param name="loginInformation"></param>
        /// <returns></returns>
        public async Task<IActionResult> AuthenticateUser(LoginFormDTO loginInformation)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AuthenticationResultDTO() { Success = false });
            }

            UserDTO? user;

            if(!_userService.AuthenticateUser(loginInformation.Username, loginInformation.Password, out user))
            {
                return Json(new AuthenticationResultDTO() { Success = false, Error = "Gebruikersnaam of wachtwoord fout"});
            }

            if (user == null || user.Id == null) 
            { 
                return Json(new AuthenticationResultDTO() { Success = false, Error = "Er gings iets fout, probeer opnieuw"});
            }

            // add neccary claims to user
            List<Claim> claims = new();
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            claims.Add(new Claim(AuthenticationConstants.FullnameClaim, $"{user.FirstName} {user.LastName}"));
            claims.Add(new Claim(AuthenticationConstants.UserIdClaim, user.Id.ToString()));
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

            return Json(new AuthenticationResultDTO() { Success = true });
        }
    }
}
