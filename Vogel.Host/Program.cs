using Autofac.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddVogelWeb(builder.Configuration);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var app = builder.Build();

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
