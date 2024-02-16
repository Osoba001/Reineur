using System.ComponentModel.DataAnnotations;

namespace Users.Application.Requests.UserRequests
{
    public class LoginRequest : TokenCommandBase
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }

    }
}
