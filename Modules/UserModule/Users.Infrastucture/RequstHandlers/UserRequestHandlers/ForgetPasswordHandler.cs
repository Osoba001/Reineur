using AuthUser.Infrastucture.Database;
using Microsoft.EntityFrameworkCore;
using Reineur.Share.EmailService;
using Reineur.Share.MediatKO;
using Users.Application.Requests.UserRequests;
using Users.Infrastucture.Mails.PasswordResetMail;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class ForgetPasswordHandler : IRequestHandler<ForgetPasswordRequest>
    {
        private readonly UserDbContext _dbContext;
        private readonly IMailGenerator<PasswordResetPayload> _mailGenerator;
        public ForgetPasswordHandler(IMailGenerator<PasswordResetPayload> mailGenerator, UserDbContext dbContext)
        {
            _mailGenerator = mailGenerator;
            _dbContext = dbContext;
        }


        public async Task<ActionResponse> HandleAsync(ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.UserTb.Where(x => x.Email == request.Email.ToLower().Trim()).FirstOrDefaultAsync();


            if (user is null)
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

            int pin = RandomPin();

            user.PasswordRecoveryPin = pin;
            user.RecoveryPinCreationTime = DateTime.UtcNow;
            _dbContext.UserTb.Entry(user).Property(x => x.PasswordRecoveryPin).IsModified = true;
            _dbContext.UserTb.Entry(user).Property(x => x.RecoveryPinCreationTime).IsModified = true;

            user.Attent++;
            _dbContext.UserTb.Entry(user).Property(x => x.Attent).IsModified = true;
            if (user.Attent > 3)
            {
                var resp = Unauthorized(InvalidEmailOrPassword);
                user.IsLock = true;
                user.LockMessage = "Due to so many attent to recover password.";
                user.Attent = 0;
                user.WhenToUnlock = DateTime.UtcNow.AddMinutes(60);
                _dbContext.UserTb.Entry(user).Property(x => x.IsLock).IsModified = true;
                _dbContext.UserTb.Entry(user).Property(x => x.LockMessage).IsModified = true;
                _dbContext.UserTb.Entry(user).Property(x => x.WhenToUnlock).IsModified = true;
                resp.Information = new { Message = user.LockMessage, user.Email, Role = user.Role.ToString(), Action = "login" };
                var rs = await _dbContext.CompleteAsync();
                if (!rs.IsSuccess)
                    return rs;
                else
                    return resp;
            }

            _ = _mailGenerator.SendAsync(new PasswordResetPayload { Receiver = request.Email, RecoveryPin = pin });
            //return new ActionResponse { Data=new { pin } };
            return SuccessResult();
        }

        private static int RandomPin()
        {
            var random = new Random();
            return random.Next(100000, 999999);
        }

    }
}
