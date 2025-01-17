﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Application.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using FluentValidation;
namespace Vogel.Content.Infrastructure.Installers
{
    public class ApplicationServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Application.AssemblyReference.Assembly))
                .RegisterFactoriesFromAssembly(Application.AssemblyReference.Assembly)
                .RegisterPoliciesHandlerFromAssembly(Application.AssemblyReference.Assembly)
                .AddValidatorsFromAssembly(Application.AssemblyReference.Assembly)
                .AddAutoMapper(Application.AssemblyReference.Assembly);

        }
    }
}
