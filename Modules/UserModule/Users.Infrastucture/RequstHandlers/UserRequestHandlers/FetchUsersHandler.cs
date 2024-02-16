using AuthUser.Application.DTOs;
using AuthUser.Infrastucture.Database;
using Microsoft.EntityFrameworkCore;
using Reineur.Share.MediatKO;
using Users.Application.Requests.UserRequests;

namespace Users.Infrastucture.RequstHandlers.UserRequestHandlers
{
    internal class FetchUsersHandler : IRequestHandler<FetchUsersRequest>
    {
        private readonly UserDbContext _dbContext;

        public FetchUsersHandler(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResponse> HandleAsync(FetchUsersRequest request, CancellationToken cancellationToken)
        {
            return new ActionResponse
            {
                PayLoad = await _dbContext.UserTb.Skip((request.PageSize - 1) * request.PageSize).Take(request.PageSize).
                Select(u => new UserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role.ToString()
                }).ToArrayAsync()
            };
        }

    }
}
