using AuthUser.Infrastucture.AuthenticationServices;
using AuthUser.Infrastucture.Database;
using Users.Application.Requests.UserRequests;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class ChangePasswordHandler(UserDbContext context, IAuthSetup authService) : IRequestHandler<ChangePasswordRequest>
    {
        private readonly IAuthSetup _authService = authService;
        private readonly UserDbContext _dbContext = context;

        public async Task<ActionResponse> HandleAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.UserTb.FindAsync(request.Id);

            if (user == null)
                return NotFoundResult(UserNotFound);

            if (user.IsLock)
            {
                if (user.Attent > 5)
                {
                    return Unauthorized($"Account is lock. \n {user.LockMessage}");
                }
                if (user.WhenToUnlock > DateTime.UtcNow)
                {
                    var waitingTime = (user.WhenToUnlock - DateTime.UtcNow).TotalMinutes;
                    return Unauthorized($"Account is lock for the nest {waitingTime}. \n {user.LockMessage}");
                }
                user.IsLock = false;
                _dbContext.UserTb.Entry(user).Property(x => x.IsLock).IsModified = true;
                await _dbContext.CompleteAsync();
            }


            if (!_authService.VerifyPassword(request.OldPassword, user.PasswordHash))
            {

                user.Attent++;
                if (user.Attent > 4)
                {
                    user.IsLock = true;
                    user.LockMessage = "Too many attent";
                    user.Attent = 0;
                    user.WhenToUnlock = DateTime.UtcNow.AddMinutes(30);
                    _dbContext.UserTb.Entry(user).Property(x => x.IsLock).IsModified = true;
                    _dbContext.UserTb.Entry(user).Property(x => x.LockMessage).IsModified = true;
                    _dbContext.UserTb.Entry(user).Property(x => x.WhenToUnlock).IsModified = true;

                }
                _dbContext.UserTb.Entry(user).Property(x => x.Attent).IsModified = true;
                await _dbContext.CompleteAsync();
                return Unauthorized(InvalidPassword);
            }

            user!.PasswordHash = _authService.HashPassword(request.NewPassword);

            _dbContext.UserTb.Entry(user).Property(x => x.PasswordHash).IsModified = true;
            return await _dbContext.CompleteAsync();
        }
    }
}