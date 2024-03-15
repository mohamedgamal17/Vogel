using Microsoft.Extensions.DependencyInjection;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Replace<TService,TImplementaion>(this IServiceCollection services)
        {
            var oldService = services.Single(x => x.ServiceType == typeof(TService));

            services.Remove(oldService);

            var serviceDescriptor = new ServiceDescriptor(typeof(TService), typeof(TImplementaion), oldService.Lifetime);

            services.Add(serviceDescriptor);

            return services;
        }
    }
}
