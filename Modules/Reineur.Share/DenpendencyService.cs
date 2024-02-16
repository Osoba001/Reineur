using Microsoft.Extensions.DependencyInjection;
using Reineur.Share.EmailService;
using Reineur.Share.MediatKO;

namespace Reineur.Share
{
    public static class DenpendencyService
    {

        public static IServiceCollection AddShareService(this IServiceCollection services)
        {
            services.AddSingleton(EnvVariable.EMAIL_CONFIGURATION);
            services.AddSingleton(EnvVariable.DEPLOYMENT_CONFIGURATION);
            services.AddScoped<IMailSender, EmailKitService>();
            services.AddScoped<IMediator, Mediator>();
            return services;
        }
    }
}
