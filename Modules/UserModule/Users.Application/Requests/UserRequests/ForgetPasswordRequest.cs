using System.ComponentModel.DataAnnotations;

namespace Users.Application.Requests.UserRequests
{

    public class ForgetPasswordRequest : Request
    {

        [EmailAddress]
        public required string Email { get; set; }

    }


}
