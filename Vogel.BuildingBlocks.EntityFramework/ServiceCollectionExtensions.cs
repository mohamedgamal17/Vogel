﻿using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.EntityFramework.Interceptors;
using Vogel.BuildingBlocks.EntityFramework.Repositories;

namespace Vogel.BuildingBlocks.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVogelEfCoreInterceptors(this IServiceCollection services)
        {
            services.AddScoped<AuditableEntityInterceptors>();

            services.AddScoped<DispatchDomainEventInterceptor>();

            services.AddTransient(typeof(IRepository<>), typeof(EFCoreRepository<>));

            return services;
        }
    }
}
