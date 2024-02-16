namespace Users.Application.Requests.UserRequests
{
    public class SetNewPasswordRequest : Request
    {

        public required string Email { get; set; }

        public required int RecoveryPin { get; set; }

        public required string Password { get; set; }
    }
}
