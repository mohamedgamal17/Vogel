using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Vogel.BuildingBlocks.Application.Behaviours;
using Vogel.BuildingBlocks.Application.Factories;
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


            return services;
        }

  
    }
}
