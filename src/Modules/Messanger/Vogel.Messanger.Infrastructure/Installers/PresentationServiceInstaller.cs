using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Endpoints;
namespace Vogel.Messanger.Infrastructure.Installers
{
    public class PresentationServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.RegisterEndpoints(Presentation.AssemblyReference.Assembly);
          

            services.RegisterMassTransitConsumers(Presentation.AssemblyReference.Assembly);
        }
    }
}
