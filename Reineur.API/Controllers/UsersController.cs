using AuthUser.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Requests.UserRequests;
using Users.Infrastucture.PermissionAuthorizations;

namespace Reineur.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController(IMediator mediator, ILogger<CustomControllerBase> logger) : CustomControllerBase(mediator, logger)
    {
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)=>
            await SendRequestAsync(request);

        [HttpGet("{page}/{pageSize}")]
        public async Task<IActionResult> FetchUsers(int page, int pageSize)=>
            await SendRequestAsync(new FetchUsersRequest { Page = page, PageSize = pageSize });

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login) =>
            await SendRequestAsync(login);

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePassword)
        {
            changePassword.Id = UserId;
            return await SendRequestAsync(changePassword);
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest forgetPassword) =>
            await SendRequestAsync(forgetPassword);


        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoveryPasswordRequest recoverPassword) =>
            await SendRequestAsync(recoverPassword);


        [HttpPost("set-new-password")]
        public async Task<IActionResult> SetNewPassword([FromBody] SetNewPasswordRequest setNewPassword) =>
            await SendRequestAsync(setNewPassword);

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest command)
        {
            return await SendRequestAsync(command);
        }


        [HasPermission(Permission.Administrator)]
        [HttpDelete("user/delete/{id}")]
        public async Task<IActionResult> HardDeleteUser(Guid id)
        {
            var command = new DeleteUserRequest { Id = id };
            command.HardDelete += Command_HardDelete;
            return await SendRequestAsync(command);
        }

        private void Command_HardDelete(object? sender, UserArgs e)
        {
           //Handle delete User's record from other modules

        }
    }
}
