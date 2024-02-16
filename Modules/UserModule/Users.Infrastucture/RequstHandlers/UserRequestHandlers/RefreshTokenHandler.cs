using AuthUser.Infrastucture.AuthenticationServices;
using AuthUser.Infrastucture.Database;
using Reineur.Share.MediatKO;
using Users.Application.Requests.UserRequests;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class RefreshTokenHandler(UserDbContext dbContext, IAuthSetup authService) : IRequestHandler<RefreshTokenRequest>
    {
        private readonly UserDbContext _dbContext = dbContext;
        private readonly IAuthSetup _authService = authService;

        public async Task<ActionResponse> HandleAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var id = _authService.RetrieveTokenNameIdentifier(request.AccessToken);
            if (id == null)
                return BadRequestResult(InvalidToken);
            var user = await _dbContext.UserTb.FindAsync(id.Value);
            if (user is null)
                return BadRequestResult(InvalidToken);

            if (request.RefreshToken != user.RefreshToken)
                return BadRequestResult(InvalidToken);
            var token = _authService.TokenManager(user);
            _dbContext.UserTb.Entry(user).Property(x => x.RefreshToken).IsModified = true;
            await _dbContext.CompleteAsync();
            return new ActionResponse
            {
                PayLoad = token
            };
        }
    }
}
