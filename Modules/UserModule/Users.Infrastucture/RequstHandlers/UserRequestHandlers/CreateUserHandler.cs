using AuthUser.Application.DTOs;
using AuthUser.Infrastucture.AuthenticationServices;
using AuthUser.Infrastucture.Database;
using Microsoft.EntityFrameworkCore;
using Reineur.Share.MediatKO;
using Users.Application.Requests.UserRequests;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class CreateUserHandler(UserDbContext dbContext, IAuthSetup authService) : IRequestHandler<CreateUserRequest>
    {
        private readonly IAuthSetup _authService = authService;
        private readonly UserDbContext _dbContext = dbContext;

        public async Task<ActionResponse> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
        {
            string email = request.Email.ToLower().Trim();
            var user = await _dbContext.UserTb.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user is not null)
                return BadRequestResult(EmailAlreadyExist);

            user = new()
            {
                Email = email,
                Role = request.Role,
                PasswordHash = _authService.HashPassword(request.Password)
            };

            _dbContext.UserTb.Add(user);
            var res = await _dbContext.CompleteAsync();
            if (!res.IsSuccess)
                return res;

            var tokenModel = _authService.TokenManager(user);
            await _dbContext.CompleteAsync();
            UserArgs resp = new() { Email = user.Email, Id = user.Id, Role = user.Role }; ;
            request.OnSoftDelete(resp);
            return new ActionResponse
            {
                PayLoad = tokenModel
            };

        }


    }
}
