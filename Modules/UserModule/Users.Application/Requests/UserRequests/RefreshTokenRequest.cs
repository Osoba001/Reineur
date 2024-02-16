using System.Text.Json.Serialization;

namespace Users.Application.Requests.UserRequests
{
    public class RefreshTokenRequest : TokenCommandBase
    {
        [JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;
        public required string AccessToken { get; set; }
    }
}
