using Auth.Application.Models;
using AuthUser.Application.DTOs;
using AuthUser.Infrastucture.Database;
using Microsoft.IdentityModel.Tokens;
using Scrypt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AuthUser.Infrastucture.AuthenticationServices
{
    internal class AuthSetup(AuthConfigData optionsConfigData) : IAuthSetup
    {

        private readonly AuthConfigData _configData = optionsConfigData;

        public string HashPassword(string password) => new ScryptEncoder().Encode(password);


        public bool VerifyPassword(string password, string hashedPassword)
            => new ScryptEncoder().Compare(password, hashedPassword);

        public TokenModel TokenManager(UserModel user)
        {

            List<Claim> claims = CreateClaims(user);

            SecurityTokenDescriptor tokenDescriptor = CreateTokenDescriptor(claims);

            TokenModel tokens = CreateToken(user, tokenDescriptor);

            return tokens;
        }

        private SecurityTokenDescriptor CreateTokenDescriptor(List<Claim> claims)
        {
            var encodedKey = Encoding.UTF8.GetBytes(_configData.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(encodedKey), SecurityAlgorithms.HmacSha256),
                Expires = DateTime.UtcNow.Add(_configData.AccessTokenLifespan),
            };
            return tokenDescriptor;
        }

        private TokenModel CreateToken(UserModel user, SecurityTokenDescriptor tokenDescriptor)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.RefreshToken = RandomToken();
            var tokens = new TokenModel()
            {
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = user.RefreshToken,
                Id = user.Id,
                TokenLifeSpanInMinutes = _configData.AccessTokenLifespan.TotalMinutes,
                Role = user.Role.ToString(),
            };
            return tokens;
        }

        private static List<Claim> CreateClaims(UserModel user)
        {
            return
            [
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.PrimarySid,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role.ToString()),
                new Claim(ClaimTypes.Name,user.Email),
            ];
        }

        private static string RandomToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public Guid? RetrieveTokenNameIdentifier(string jwt)
        {
            try
            {
                if (!VerifyExpiredJwtToken(jwt))
                    return null;
                var payload = jwt.Split('.')[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
                if (keyValuePairs.TryGetValue("nameid", out var nameIdValue))
                {
                    if (Guid.TryParse(nameIdValue.ToString(), out var value))
                        return value;
                    return null;

                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public bool VerifyExpiredJwtToken(string jwtToken)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configData.SecretKey)),
                    ValidateIssuerSigningKey = true,
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}
