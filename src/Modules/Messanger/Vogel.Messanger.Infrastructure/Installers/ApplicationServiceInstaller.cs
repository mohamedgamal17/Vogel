﻿using MassTransit;
using MassTransit.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Vogel.BuildingBlocks.Application.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
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

            services.RegisterMassTransitConsumers(Application.AssemblyReference.Assembly);

        }

    }
}
