﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Application.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Social.MongoEntities;
namespace Vogel.Social.Infrastructure.Installers
{
    public class ApplicationServiceInstaller : IServiceInstaller
    {
        public void InstallAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AssemblyReference.Assembly))
                .RegisterFactoriesFromAssembly(AssemblyReference.Assembly)
                .RegisterPoliciesHandlerFromAssembly(AssemblyReference.Assembly);

        }
    }
}