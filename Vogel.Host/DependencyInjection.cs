using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Vogel.Application;
using Vogel.Host.Infrastructure;
using Vogel.Infrastructure;
namespace Vogel.Host
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddVogelWeb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplication(configuration);

            services.AddInfrastructure(configuration);

            services.AddProblemDetails(opt =>
            {
                opt.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            }).AddControllers()
            .AddProblemDetailsConventions();

            services.AddEndpointsApiExplorer();

            services.AddHttpContextAccessor();

            ConfigureAuthentication(services, configuration);

            ConfigureSwagger(services, configuration);

            return services;
        }


        public static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = configuration.GetValue<string>("IdentityProvider:Authority");
                options.Audience = configuration.GetValue<string>("IdentityProvider:Audience");
            });

            services.AddAuthorization();

        }


        public static void ConfigureSwagger(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<AuthorizeCheckOperationFilter>();
                options.CustomSchemaIds(x => x.FullName);
                options.DocInclusionPredicate((docName, description) => true);
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vogel Api",
                    Version = "v1",
                    Description = "Vogel api ",

                });
                options.ResolveConflictingActions(x => x.First());
                bool hasSwaggerClient = configuration.GetChildren().Any(item => item.Key == "SwaggerClient");

                if (hasSwaggerClient)
                {
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        BearerFormat = "JWT",
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(configuration.GetValue<string>("SwaggerClient:AuthorizationEndPoint")!),
                                TokenUrl = new Uri(configuration.GetValue<string>("SwaggerClient:TokenEndPoint")!),
                                Scopes = configuration.GetSection("SwaggerClient:Scopes").GetChildren()
                                    .ToDictionary(x => x.Key, x => x.Value)
                            },

                        },
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        Description = "OpenId Security Scheme"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                            },
                            new[] { "openid" }
                        }
                    });
                }
            });
        }
    }
}
