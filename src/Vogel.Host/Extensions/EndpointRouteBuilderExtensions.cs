using CaseConverter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Reflection;
using Vogel.BuildingBlocks.Infrastructure.SignalR;

namespace Vogel.Host.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void MapDynamicHubs(this IEndpointRouteBuilder endpoints, params Assembly[] modules)
        {
            var registery =  endpoints.ServiceProvider.GetService<SignalRRegistery>();

            if(registery != null)
            {
                foreach (var hubType in registery.HubTypes)
                {
                    var route = ExtractHubRoute(hubType);

                    var mapHubMethod = typeof(HubEndpointRouteBuilderExtensions).GetMethod("MapHub", new[] { typeof(IEndpointRouteBuilder), typeof(string) });

                    var genericMapHub = mapHubMethod!.MakeGenericMethod(hubType);

                    genericMapHub.Invoke(null, new object[] { endpoints, route });
                }
            }
        }
        private static string ExtractHubRoute(Type hubType)
        {
            var routeAttribute = hubType.GetCustomAttribute<RouteAttribute>();

            var hubRoute = $"/hubs/{routeAttribute?.Name ?? hubType.Name.ToCamelCase()}";

            return hubRoute;
        }
    }
}
