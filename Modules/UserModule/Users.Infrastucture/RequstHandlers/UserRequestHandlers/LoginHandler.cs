using AuthUser.Infrastucture.AuthenticationServices;
using AuthUser.Infrastucture.Database;
using Microsoft.EntityFrameworkCore;
using Reineur.Share.MediatKO;
using Users.Application.Requests.UserRequests;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class LoginHandler(UserDbContext dbContext, IAuthSetup authService) : IRequestHandler<LoginRequest>
    {
        private readonly UserDbContext _dbContext = dbContext;
        private readonly IAuthSetup _authService = authService;

        public async Task<ActionResponse> HandleAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.UserTb.FirstOrDefaultAsync(x => x.Email == request.Email.ToLower().Trim());

            if (user is null)
                return Unauthorized(InvalidEmailOrPassword);

            if (user.IsLock)
            {
                if (user.Attent > 10)
                {
                    return Unauthorized($"This action is disable. \n {user.LockMessage}");
                }
                if (user.WhenToUnlock > DateTime.UtcNow)
                {
                    var waitingTime = (user.WhenToUnlock - DateTime.UtcNow).TotalMinutes;
                    return Unauthorized($"This action is disable for the nest {waitingTime} minutes. \n {user.LockMessage}");
                }
                user.IsLock = false;
                _dbContext.UserTb.Entry(user).Property(x => x.IsLock).IsModified = true;
                await _dbContext.CompleteAsync();
            }

            if (!_authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                var resp = Unauthorized(InvalidEmailOrPassword);
                user.Attent++;
                if (user.Attent > 9)
                {
                    user.IsLock = true;
                    user.LockMessage = "Due to so many login attent.";
                    user.Attent = 0;
                    user.WhenToUnlock = DateTime.UtcNow.AddMinutes(15);
                    _dbContext.UserTb.Entry(user).Property(x => x.IsLock).IsModified = true;
                    _dbContext.UserTb.Entry(user).Property(x => x.LockMessage).IsModified = true;
                    _dbContext.UserTb.Entry(user).Property(x => x.WhenToUnlock).IsModified = true;
                    resp.Information = new { Message = user.LockMessage, user.Email, Role = user.Role.ToString(), Action = "login" };
                }
                _dbContext.UserTb.Entry(user).Property(x => x.Attent).IsModified = true;
                await _dbContext.CompleteAsync();
                return resp;
            }

            var tokenModel = _authService.TokenManager(user);

            _dbContext.UserTb.Entry(user).Property(x => x.RefreshToken).IsModified = true;
            await _dbContext.CompleteAsync();

            return new ActionResponse
            {
                PayLoad = tokenModel
            };
        }
    }
}
