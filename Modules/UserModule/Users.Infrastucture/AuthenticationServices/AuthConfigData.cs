
namespace Auth.Application.Models
{
    public class AuthConfigData
    {
        public readonly string SecretKey = EnvVariable.AUTH_SECRET_KEY;
        public readonly TimeSpan AccessTokenLifespan = EnvVariable.ACCESS_TOKEN_TTL;

    }

}
