using System.Reflection;
using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.BuildingBlocks.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InstallServiceFromAssembly(this IServiceCollection services  , IConfiguration configuration , IHostEnvironment hostEnvironment ,params Assembly[] assemblies)
        {
            if(assemblies == null || assemblies.Count() < 1)
            {
                return services;
            }

            foreach (var assembly in assemblies)
            {
               var types = assembly.GetTypes()
                    .Where(x => x.IsClass && x.IsAssignableTo(typeof(IServiceInstaller)))
                    .ToList();

                ResolveServicesInstallers(types, services, configuration, hostEnvironment);
            }

            return services;
        }

        public static IServiceCollection InstallModulesFromAssembly(this IServiceCollection services, IConfiguration configuration ,IHostEnvironment hostEnvironment,  params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Count() < 1)
            {
                return services;
            }

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                     .Where(x => x.IsClass && x.IsAssignableTo(typeof(IModuleInstaller)))
                     .ToList();

                ResolveModulesInstallers(types, services, configuration, hostEnvironment);
            }

            return services;
        }

        public static IServiceCollection Replace<TService, TImplementaion>(this IServiceCollection services)
        {
            return services.Replace(typeof(TService), typeof(TImplementaion));
        }

        public static IServiceCollection Replace(this IServiceCollection services,Type service, Type implementaion)
        {
            var oldService = services.Single(x => x.ServiceType == service);

            services.Remove(oldService);

            var serviceDescriptor = new ServiceDescriptor(service, service, oldService.Lifetime);

            services.Add(serviceDescriptor);

            return services;
        }

        private static void ResolveServicesInstallers(IEnumerable<Type> types , IServiceCollection services,IConfiguration configuration , IHostEnvironment hostEnvironment)
        {
            foreach (var type in types)
            {
                var obj = (IServiceInstaller) (Activator.CreateInstance(type , new object[] {})!);

                obj.Install(services, configuration, hostEnvironment);
            }
        }

        private static void ResolveModulesInstallers(IEnumerable<Type> types, IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            foreach (var type in types)
            {
                var obj = (IModuleInstaller)(Activator.CreateInstance(type, new object[] { })!);

                obj.Install(services, configuration, hostEnvironment);
            }
        }
    }
}
