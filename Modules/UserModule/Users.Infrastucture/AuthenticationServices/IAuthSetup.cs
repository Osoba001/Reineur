using AuthUser.Application.DTOs;
using AuthUser.Infrastucture.Database;

namespace AuthUser.Infrastucture.AuthenticationServices
{

    internal interface IAuthSetup
    {
        TokenModel TokenManager(UserModel user);

        string HashPassword(string password);

        bool VerifyPassword(string password, string hashedPassword);
        Guid? RetrieveTokenNameIdentifier(string jwt);
    }
}
