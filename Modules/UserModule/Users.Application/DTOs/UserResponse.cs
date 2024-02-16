namespace AuthUser.Application.DTOs
{
    public class UserResponse
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }


    }
}
