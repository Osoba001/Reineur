using System.ComponentModel.DataAnnotations;

namespace Users.Application.Requests.UserRequests
{

    public class RecoveryPasswordRequest : Request
    {
        [EmailAddress]
        public required string Email { get; set; }

        public required int RecoveryPin { get; set; }

    }
}
