using System.ComponentModel.DataAnnotations;

namespace SeniorConnectMessengerWeb.Models.DTO.Auth
{
    public class LoginFormDTO
    {
        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        public string? ReturnUrl = "";
    }
}
