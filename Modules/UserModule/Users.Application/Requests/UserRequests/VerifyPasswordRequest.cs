namespace Users.Application.Requests.UserRequests
{
    public class VerifyPasswordRequest : Request
    {
        public required string Password { get; set; }
    }
}
