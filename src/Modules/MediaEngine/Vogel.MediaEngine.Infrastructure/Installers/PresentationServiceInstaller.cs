using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Endpoints;
using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.MediaEngine.Infrastructure.Installers
{
    public class PresentationServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.RegisterEndpoints(Presentation.AssemblyReference.Assembly);
        }
    }
}
