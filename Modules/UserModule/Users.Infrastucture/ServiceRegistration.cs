using Auth.Application.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using AuthUser.Infrastucture.AuthenticationServices;
using AuthUser.Infrastucture.Database;
using Reineur.Share.EmailService;
using Microsoft.EntityFrameworkCore;
using Users.Infrastucture.PermissionAuthorizations;
using Users.Infrastucture.Mails.PasswordResetMail;

namespace Users.Infrastucture
{
    public static class ServiceRegistration
    {
        public static IServiceCollection UserServiceCollection(this IServiceCollection services)
        {
            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(EnvVariable.USER_DB_CONNECT_STRING));



            var authConf = new AuthConfigData();
            services.AddSingleton(authConf);
            services.AddScoped<IAuthSetup, AuthSetup>();
            services.JWTService(authConf.SecretKey);
            services.AddAuthorizationPolicy();
            services.AddTransient<IMailGenerator<PasswordResetPayload>, PasswordResetMailGenerator>();
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisteredAssemblyHandlers(assembly);
            //var provider = services.BuildServiceProvider();
            //var context = provider.GetService<SwinvaAuthDbContext>();
            //context?.Database.Migrate();
            return services;
        }

        private static IServiceCollection JWTService(this IServiceCollection services, string secretKey)
        {
            var key = Encoding.UTF8.GetBytes(secretKey);
            services.AddAuthentication().AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            return services;
        }
    }
}
