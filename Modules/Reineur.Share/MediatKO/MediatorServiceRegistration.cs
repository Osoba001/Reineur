using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Reineur.Share.MediatKO
{
    public static class MediatorServiceRegistration
    {
        public static IServiceCollection RegisteredAssemblyHandlers(this IServiceCollection services, Assembly assembly)
        {
            var requestHandlerType = typeof(IRequestHandler<>);
            var requestHandlerImplementations = assembly.GetTypes()
                .Where(type => type.GetInterfaces()
                    .Any(interfaceType => interfaceType.IsGenericType &&
                                         interfaceType.GetGenericTypeDefinition() == requestHandlerType))
                .ToList();

            foreach (var implementationType in requestHandlerImplementations)
            {
                var requestType = implementationType.GetInterfaces()
                    .First(interfaceType => interfaceType.IsGenericType &&
                                           interfaceType.GetGenericTypeDefinition() == requestHandlerType)
                    .GetGenericArguments()[0];

                var closedGenericHandlerType = requestHandlerType.MakeGenericType(requestType);
                services.AddTransient(closedGenericHandlerType, implementationType);
            }
            return services;
        }
    }

}

