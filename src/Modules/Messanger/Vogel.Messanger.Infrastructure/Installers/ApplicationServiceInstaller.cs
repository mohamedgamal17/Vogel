using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Vogel.BuildingBlocks.Application.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
namespace Vogel.Messanger.Infrastructure.Installers
{
    public class ApplicationServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Application.AssemblyReference.Assembly))
                .RegisterFactoriesFromAssembly(Application.AssemblyReference.Assembly)
                .RegisterPoliciesHandlerFromAssembly(Application.AssemblyReference.Assembly)
                .AddAutoMapper(Application.AssemblyReference.Assembly);

            RegisterMassTransitConsumers(services);

        }


        private void RegisterMassTransitConsumers(IServiceCollection services)
        {
            var consumerTypes = Application.AssemblyReference.Assembly.GetClassesThatImplementing<IConsumer>();


            var registerConsumer = typeof(DependencyInjectionConsumerRegistrationExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m =>
                    m.Name == "RegisterConsumer" &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters().Length == 1);


            foreach (var consumerType in consumerTypes)
            {
                registerConsumer.MakeGenericMethod(consumerType).Invoke(services, [services]);
            }
        }
    }
}
