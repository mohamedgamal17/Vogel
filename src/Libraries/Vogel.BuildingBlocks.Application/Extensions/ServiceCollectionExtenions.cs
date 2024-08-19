using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Vogel.BuildingBlocks.Application.Factories;

namespace Vogel.BuildingBlocks.Application.Extensions
{
    public static class ServiceCollectionExtenions
    {
        public static IServiceCollection RegisterFactoriesFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
            .Where(x => x.IsClass)
            .Where(x => x.GetInterfaces().Any(c => c == typeof(IResponseFactory)))
            .ToList();

            foreach (var type in types)
            {
                services.AddTransient(type.GetInterfaces().First(), type);
            }

            return services;
        }

        public static IServiceCollection RegisterPoliciesHandlerFromAssembly(this IServiceCollection services , Assembly assembly)
        {
            var types = assembly.GetTypes()
               .Where(x => x.IsClass)
               .Where(x => x.GetInterfaces().Any(i => i == typeof(IAuthorizationHandler)))
               .ToList();

            foreach (var type in types)
            {
                services.AddTransient(type.GetInterfaces().First(), type);
            }

            return services;
        }
    }
}
