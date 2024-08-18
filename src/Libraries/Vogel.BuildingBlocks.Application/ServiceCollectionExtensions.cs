using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Vogel.BuildingBlocks.Application.Behaviours;
using Vogel.BuildingBlocks.Application.Factories;
using Vogel.BuildingBlocks.Application.Security;
namespace Vogel.BuildingBlocks.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVogelCore(this IServiceCollection services, Assembly? assembly = null)
        {
            services.AddMediatR(cfg =>
            {
                if (assembly != null)
                {
                    cfg.RegisterServicesFromAssemblies(assembly);
                }

                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            });

            services.AddTransient<IApplicationAuthorizationService, ApplicationAuthorizationService>();

            services.AddTransient<ISecurityContext, SecurityContext>();

            if(assembly != null)
            {
                RegisterAuthorizationPolicyHandlers(services, assembly);

                RegisterResponseFactories(services, assembly);
            }

            return services;
        }

        private static void RegisterAuthorizationPolicyHandlers(IServiceCollection services , Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(x => x.IsClass)
                .Where(x => x.GetInterfaces().Any(i => i == typeof(IAuthorizationHandler)))
                .ToList();

            foreach (var type in types)
            {
                services.AddTransient(type.GetInterfaces().First(), type);
            }

        }
        private static void RegisterResponseFactories(IServiceCollection services, Assembly assembly)
        {

            var types = assembly.GetTypes()
                .Where(x => x.IsClass)
                .Where(x => x.GetInterfaces().Any(c => c == typeof(IResponseFactory)))
                .ToList();

            foreach (var type in types)
            {
                services.AddTransient(type.GetInterfaces().First(), type);
            }

        }

    }
}
