
namespace Users.Application.Requests.UserRequests
{
    public class ChangePasswordRequest : Request
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }

        public override ActionResponse Validate()
        {
            if (NewPassword.Length < 8) return BadRequestResult("New password is too short.");
            return SuccessResult();
        }

    }
}