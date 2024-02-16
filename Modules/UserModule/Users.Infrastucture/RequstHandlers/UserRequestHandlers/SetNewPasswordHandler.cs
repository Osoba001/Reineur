using AuthUser.Infrastucture.AuthenticationServices;
using AuthUser.Infrastucture.Database;
using Microsoft.EntityFrameworkCore;
using Reineur.Share.MediatKO;
using Users.Application.Requests.UserRequests;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class SetNewPasswordHandler(UserDbContext dbContext, IAuthSetup authService) : IRequestHandler<SetNewPasswordRequest>
    {
        private readonly UserDbContext _dbContext = dbContext;
        private readonly IAuthSetup _authService = authService;

        public async Task<ActionResponse> HandleAsync(SetNewPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.UserTb.FirstOrDefaultAsync(x => x.Email == request.Email.ToLower().Trim());

            if (user is null)
                return NotFoundResult(UserNotFound);
            if (user.PasswordRecoveryPin != request.RecoveryPin)
                return BadRequestResult(IncorrectPin);

            if (user.AllowSetNewPassword < DateTime.UtcNow.AddMinutes(-10))
                return BadRequestResult(SessionExpired);

            user.PasswordHash = _authService.HashPassword(request.Password);
            user.AllowSetNewPassword = DateTime.UtcNow.AddMinutes(-20);
            _dbContext.UserTb.Entry(user).Property(x => x.PasswordHash).IsModified = true;
            _dbContext.UserTb.Entry(user).Property(x => x.AllowSetNewPassword).IsModified = true;
            return await _dbContext.CompleteAsync();
        }

    }
}
