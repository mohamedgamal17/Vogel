using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.EntityFramework.Interceptors;

namespace Vogel.BuildingBlocks.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDbContextInterceptors(this IServiceCollection services)
        {
            services.AddScoped<AuditableEntityInterceptors>();

            services.AddScoped<DispatchDomainEventInterceptor>();

            return services;
        }
    }
}
