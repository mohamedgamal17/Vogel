using System.Reflection;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.Infrastructure.SignalR;
namespace Vogel.BuildingBlocks.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InstallServiceFromAssembly(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment, params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Count() < 1)
            {
                return services;
            }

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                     .Where(x => x.IsClass && x.IsAssignableTo(typeof(IServiceInstaller)))
                     .ToList();

                types.ForEach((t) => services.InstallService(t, configuration, hostEnvironment));
            }

            return services;
        }

        public static IServiceCollection InstallService<TService>(this IServiceCollection services, IConfiguration configuration , IHostEnvironment hostEnvironment)
        {
            return services.InstallService(typeof(TService), configuration, hostEnvironment);
        }

        public static IServiceCollection InstallService(this IServiceCollection services , Type serviceType, IConfiguration configuration , IHostEnvironment hostEnvironment)
        {
            if (serviceType.IsAssignableFrom(typeof(IServiceInstaller)))
            {
                throw new InvalidOperationException($"[{serviceType.AssemblyQualifiedName}] must implement type of [{typeof(IServiceInstaller).AssemblyQualifiedName}]");
            }


            var hasEmptyConstructor = serviceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                 .Where(x => x.GetParameters().Length == 0)
                 .Any();

            if (!hasEmptyConstructor)
            {
                throw new InvalidOperationException($"[{serviceType.AssemblyQualifiedName}] must have parameterless constructor to be able to install module");
            }

            var obj = (IServiceInstaller)(Activator.CreateInstance(serviceType, new object[] { })!);

            obj.Install(services, configuration, hostEnvironment);

            return services;
        }

        public static IServiceCollection InstallModulesFromAssembly(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment, params Assembly[] assemblies)
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

                types.ForEach((t) => services.InstallModule(t, configuration, hostEnvironment));
            }

            return services;
        }

        public static IServiceCollection InstallModule<TModule>(this IServiceCollection services , IConfiguration configuration, IHostEnvironment hostEnvironment)
            where TModule : IModuleInstaller
        {
            return services.InstallModule(typeof(TModule), configuration, hostEnvironment);
        }

        public static IServiceCollection InstallModule(this IServiceCollection services, Type moduleType , IConfiguration configuration , IHostEnvironment hostEnvironment)
        {
            if (moduleType.IsAssignableFrom(typeof(IModuleInstaller)))
            {
                throw new InvalidOperationException($"[{moduleType.AssemblyQualifiedName}] must implement type of [{typeof(IModuleInstaller).AssemblyQualifiedName}]");
            }

            var hasEmptyConstructor = moduleType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                 .Where(x => x.GetParameters().Length == 0)
                 .Any();

            if (!hasEmptyConstructor)
            {
                throw new InvalidOperationException($"[{moduleType.AssemblyQualifiedName}] must have parameterless constructor to be able to install module");
            }

           var obj = (IModuleInstaller)(Activator.CreateInstance(moduleType, new object[] { })!);

            obj.Install(services, configuration, hostEnvironment);

            return services;
        }

        public static IServiceCollection Replace<TService, TImplementaion>(this IServiceCollection services)
        {
            return services.Replace(typeof(TService), typeof(TImplementaion));
        }

        public static IServiceCollection Replace(this IServiceCollection services, Type service, Type implementaion)
        {
            var oldService = services.Single(x => x.ServiceType == service);

            services.Remove(oldService);

            var serviceDescriptor = new ServiceDescriptor(service, implementaion, oldService.Lifetime);

            services.Add(serviceDescriptor);

            return services;
        }

        public static T? GetSinglatonOrNull<T>(this IServiceCollection services)
        {
            return (T)services.GetSinglatonOrNull(typeof(T));
        }
        public static object? GetSinglatonOrNull(this IServiceCollection services , Type targetType)
        {
            var descriptor = services.FirstOrDefault(
                d => d.ServiceType == targetType && d.Lifetime == ServiceLifetime.Singleton);

            return descriptor?.ImplementationInstance;
        }

        public static IServiceCollection ConfigureSignalRRegistery(this IServiceCollection services , Action<SignalRRegistery> options) 
        {
            var registery = services.GetSinglatonOrNull<SignalRRegistery>();
            
            if(registery == null)
            {
                registery = new SignalRRegistery();

                services.AddSingleton(registery);
            }

            options.Invoke(registery);

            return services;
        }

        private static void ResolveServicesInstallers(IEnumerable<Type> types, IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            foreach (var type in types)
            {
                var obj = (IServiceInstaller)(Activator.CreateInstance(type, new object[] { })!);

                obj.Install(services, configuration, hostEnvironment);
            }
        }

    }
}
