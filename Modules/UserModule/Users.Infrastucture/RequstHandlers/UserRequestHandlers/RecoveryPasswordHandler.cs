using AuthUser.Infrastucture.Database;
using Microsoft.EntityFrameworkCore;
using Reineur.Share.MediatKO;
using Users.Application.Requests.UserRequests;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class RecoveryPasswordHandler(UserDbContext dbContext) : IRequestHandler<RecoveryPasswordRequest>
    {
        private readonly UserDbContext _dbContext = dbContext;

        public async Task<ActionResponse> HandleAsync(RecoveryPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.UserTb.FirstOrDefaultAsync(x => x.Email == request.Email.ToLower().Trim());

            if (user is null)
                return NotFoundResult(UserNotFound);

            if (user.RecoveryPinCreationTime.AddMinutes(10) < DateTime.UtcNow)
                return BadRequestResult(SessionExpired);

            if (user.PasswordRecoveryPin != request.RecoveryPin)
            {
                var result = BadRequestResult(IncorrectPin);
                user.RecoveryPinCreationTime = DateTime.UtcNow.AddMinutes(-20);
                _dbContext.UserTb.Entry(user).Property(x => x.RecoveryPinCreationTime).IsModified = true;
                await _dbContext.CompleteAsync();
                return result;
            }
            user.AllowSetNewPassword = DateTime.UtcNow;
            _dbContext.UserTb.Entry(user).Property(x => x.AllowSetNewPassword).IsModified = true;
            return await _dbContext.CompleteAsync();
        }
    }
}
