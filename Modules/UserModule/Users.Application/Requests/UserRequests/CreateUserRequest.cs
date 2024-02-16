using AuthUser.Application.Constants;
using AuthUser.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Users.Application.Requests.UserRequests
{
    public partial class CreateUserRequest : Request
    {
        [JsonIgnore]
        public Role Role { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required string Password { get; set; }

        

        public event EventHandler<UserArgs>? CreatedUser;
        public virtual void OnSoftDelete(UserArgs args)
        {
            CreatedUser?.Invoke(this, args);
        }
        public override ActionResponse Validate()
        {
            Email = Email.Trim().ToLower();
            return base.Validate();
        }

    }
}
