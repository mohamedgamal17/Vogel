using Autofac.Extensions.DependencyInjection;
using FastEndpoints;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Host;
using FastEndpoints.Swagger;
using Vogel.Host.Extensions;
using System.Reflection;
var builder = WebApplication.CreateBuilder();

builder.InstallModule<HostModuleInstaller>();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var app = builder.Build();


if (builder.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.UseHttpsRedirection()
    .UseCors(bld =>
        bld
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    )
    .UseAuthentication()
    .UseAuthorization()
    .UseRouting()
    .UseEndpoints(endpoint =>
    {
        endpoint.MapFastEndpoints();
        endpoint.MapDynamicHubs();
    });
    

await app.RunModulesBootstrapperAsync();

app.Run();



