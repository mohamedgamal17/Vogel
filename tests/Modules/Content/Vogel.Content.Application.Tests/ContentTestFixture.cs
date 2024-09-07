﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.Application.Tests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;

namespace Vogel.Content.Application.Tests
{
    [TestFixture]
    public class ContentTestFixture : TestFixture
    {
        protected override Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.InstallModule<ContentApplicationTestModuleInstaller>(configuration, hostEnvironment);
            return Task.CompletedTask;
        }
        protected override async Task InitializeAsync(IServiceProvider services)
        {
            await services.RunModulesBootstrapperAsync();
        }
        protected override async Task ShutdownAsync(IServiceProvider services)
        {
            await DropSqlDb();
            await DropMongoDb();
        }
    }
}