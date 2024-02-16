using AuthUser.Application.DTOs;
using AuthUser.Infrastucture.Database;
using Reineur.Share.MediatKO;
using Users.Application.Requests.UserRequests;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class DeleteUserHandler(UserDbContext dbContext) : IRequestHandler<DeleteUserRequest>
    {
        private readonly UserDbContext _dbContext = dbContext;

        public async Task<ActionResponse> HandleAsync(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.UserTb.FindAsync(request.Id);
            if (user is null)
                return NotFoundResult(UserNotFound);

            _dbContext.UserTb.Remove(user);
            var res = await _dbContext.CompleteAsync();
            if (!res.IsSuccess)
                return res;

            UserArgs resp = new() { Email = user.Email, Id = user.Id, Role = user.Role }; ;
            request.OnHardDelete(resp);
            return SuccessResult();
        }

    }
}
