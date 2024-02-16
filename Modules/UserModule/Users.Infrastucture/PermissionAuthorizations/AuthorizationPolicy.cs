using AuthUser.Application.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Infrastucture.PermissionAuthorizations
{
    public static class AuthorizationPolicy
    {
        public static IServiceCollection AddAuthorizationPolicy(this IServiceCollection services)
        {
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy(Permission.Administrator.ToString(),
                    policy => policy.RequireRole(Role.Admin.ToString(),
                    Role.SuperAdmin.ToString()));

                opts.AddPolicy(Permission.SuperAdministrator.ToString(),
                   policy => policy.RequireRole(Role.SuperAdmin.ToString()));

                opts.AddPolicy(Permission.User.ToString(),
                  policy => policy.RequireRole(Role.User.ToString(),
                  Role.SuperAdmin.ToString()));
            });
            return services;
        }
    }
}
