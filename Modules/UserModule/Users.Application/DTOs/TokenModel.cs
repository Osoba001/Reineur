using System.Text.Json.Serialization;

namespace AuthUser.Application.DTOs
{
    public class TokenModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string Role { get; set; }
        public Guid Id { get; set; }
        public double TokenLifeSpanInMinutes { get; set; }
    }

}
