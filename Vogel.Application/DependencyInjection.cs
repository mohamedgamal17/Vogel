using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Vogel.Application.Common.Behaviours;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Factories;
using Vogel.Application.Medias.Policies;
using Vogel.Application.Posts.Policies;

namespace Vogel.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehaviour<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            });

            ConfigureAuthroizationPolicies(services);

            RegiesterResponseFactories(services);

            return services;
        }
        private static void ConfigureAuthroizationPolicies(IServiceCollection services)
        {
            services.AddTransient<IAuthorizationHandler, PostOperationAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, IsMediaOwnerAuthorizationHandler>();
        }

        private static void RegiesterResponseFactories(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes()
                .Where(x=> x.IsClass)
                .Where(x => x.GetInterfaces().Any(c => c == typeof(IResponseFactory)))
                .ToList();

            foreach (var type in types)
            {
                services.AddTransient(type.GetInterfaces().First(), type);
            }

        }

    }
}
