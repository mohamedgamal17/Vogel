using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
namespace Vogel.Social.Infrastructure.Installers
{
    public class PresentationServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddFastEndpoints(opt =>
            {
                opt.DisableAutoDiscovery = true;
                opt.Assemblies = new Assembly[] { Presentation.AssemblyReference.Assembly };
            });
        }
    }
}
