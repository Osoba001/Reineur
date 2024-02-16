using AuthUser.Application.DTOs;

namespace Users.Application.Requests.UserRequests
{
    public class DeleteUserRequest : Request
    {
        public event EventHandler<UserArgs>? HardDelete;

        public virtual void OnHardDelete(UserArgs user)
        {
            HardDelete?.Invoke(this, user);
        }
    }
}
