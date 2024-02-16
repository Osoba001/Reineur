using Microsoft.AspNetCore.Authorization;

namespace Users.Infrastucture.PermissionAuthorizations
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permission permission) : base(policy: permission.ToString())
        {

        }
    }
}
