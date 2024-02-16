using AuthUser.Infrastucture.AuthenticationServices;
using AuthUser.Infrastucture.Database;
using Users.Application.Requests.UserRequests;


namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class VerifyPasswordHandler(UserDbContext dbContext, IAuthSetup authSetup) : IRequestHandler<VerifyPasswordRequest>
    {
        private readonly UserDbContext _dbContext = dbContext;
        private readonly IAuthSetup _authSetup = authSetup;

        public async Task<ActionResponse> HandleAsync(VerifyPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.UserTb.FindAsync(request.Id);

            if (user == null)
                return NotFoundResult(UserNotFound);

            if (user.IsLock)
            {
                if (user.Attent > 5)
                {
                    return Unauthorized($"This action is disable. \n {user.LockMessage}");
                }
                if (user.WhenToUnlock > DateTime.UtcNow)
                {
                    var waitingTime = (user.WhenToUnlock - DateTime.UtcNow).TotalMinutes;
                    return Unauthorized($"This action is disable for the nest {waitingTime}minutes. \n {user.LockMessage}");
                }
                user.IsLock = false;
                _dbContext.UserTb.Entry(user).Property(x => x.IsLock).IsModified = true;
                await _dbContext.CompleteAsync();
            }


            if (!_authSetup.VerifyPassword(request.Password, user.PasswordHash))
            {
                var resp = BadRequestResult(InvalidPassword);
                user.Attent++;
                if (user.Attent > 4)
                {
                    user.IsLock = true;
                    user.LockMessage = "Due to so many attent to verify password.";
                    user.Attent = 0;
                    user.WhenToUnlock = DateTime.UtcNow.AddMinutes(60);
                    _dbContext.UserTb.Entry(user).Property(x => x.IsLock).IsModified = true;
                    _dbContext.UserTb.Entry(user).Property(x => x.LockMessage).IsModified = true;
                    _dbContext.UserTb.Entry(user).Property(x => x.WhenToUnlock).IsModified = true;
                    resp.Information = new { Message = user.LockMessage, user.Email, Role = user.Role.ToString(), Action = "login" };
                }
                _dbContext.UserTb.Entry(user).Property(x => x.Attent).IsModified = true;
                await _dbContext.CompleteAsync();
                return resp;
            }
            return SuccessResult();
        }
    }
}
