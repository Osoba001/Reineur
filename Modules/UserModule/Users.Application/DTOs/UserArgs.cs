using AuthUser.Application.Constants;

namespace AuthUser.Application.DTOs
{

    public class UserArgs : EventArgs
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required Role Role { get; set; }

    }
}
