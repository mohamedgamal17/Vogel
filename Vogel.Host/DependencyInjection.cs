using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddVogelWeb(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddProblemDetails(opt =>
            {
                opt.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            }).AddControllers()
            .AddProblemDetailsConventions();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            return services;
        }
    }
}
