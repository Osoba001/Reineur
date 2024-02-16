using System.Text.Json.Serialization;

namespace Users.Application.Requests.UserRequests
{
    public class LogoutRequest : Request
    {
        [JsonIgnore]
        public override Guid Id { get; set; }
    }
}
