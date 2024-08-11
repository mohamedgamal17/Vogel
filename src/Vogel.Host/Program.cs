using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Vogel.BuildingBlocks.MongoDb.Migrations;
using Vogel.Host;
using Vogel.Infrastructure.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddVogelWeb(builder.Configuration);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var app = builder.Build();


await MigrateSqlDatabase(app.Services);

await MigrateMongoDatabase(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var config = app.Configuration;
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Vogel API");
        options.OAuthClientId(config.GetValue<string>("SwaggerClient:ClientId"));
        options.OAuthClientSecret(config.GetValue<string>("SwaggerClient:ClientSecret"));
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseCors(builder=>
    builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


async Task MigrateSqlDatabase(IServiceProvider serviceProvider)
{
    var dbcontext = serviceProvider.GetRequiredService<ApplicationDbContext>();

    await dbcontext.Database.MigrateAsync();
}

async Task MigrateMongoDatabase(IServiceProvider serviceProvider)
{
    var mongoEngine = serviceProvider.GetRequiredService<IMongoMigrationEngine>();

    await mongoEngine.MigrateAsync();
}