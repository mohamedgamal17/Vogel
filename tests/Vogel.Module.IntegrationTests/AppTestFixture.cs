using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Vogel.Module.IntegrationTests
{
    [SetUpFixture]
    public class AppTestFixture<TProgram> where TProgram : class
    {
        public IServiceProvider ServiceProvider => _app.Services;
        public TestServer TestServer => _app.Server;
        public HttpClient Client { get; private set; } = null!;

        private WebApplicationFactory<TProgram> _app = null!;

        protected virtual void ConfigureService(IServiceCollection services)
        {

        }

        protected virtual void ConfigureApp(IWebHostBuilder a) { }
        protected virtual IHost ConfigureAppHost(IHostBuilder a)
        {
            var host = a.Build();

            host.Start();

            return host;
        }

        protected ValueTask SetupAsync()
            => ValueTask.CompletedTask;


        protected ValueTask ShutdownAsync()
            => ValueTask.CompletedTask;


        [OneTimeSetUp]
        public ValueTask InitializeAsync()
        {
            _app = WafInitializer();

            Client = _app.CreateClient();

            WebApplicationFactory<TProgram> WafInitializer()
            {
                return new WafWrapper(ConfigureAppHost)
                    .WithWebHostBuilder(
                        b =>
                        {
                            b.UseEnvironment("Testing");
                            b.ConfigureTestServices(ConfigureService);
                            ConfigureApp(b);
                        }

               );
            }

            return ValueTask.CompletedTask;
        }


        [OneTimeTearDown]
        public  async ValueTask TearDownAsync()
        {
            await ShutdownAsync();

            await _app.DisposeAsync();

            Client?.Dispose();
        }

        class WafWrapper(Func<IHostBuilder, IHost> configureAppHost) : WebApplicationFactory<TProgram>
        {
            protected override IHost CreateHost(IHostBuilder builder)
                => configureAppHost(builder);
        }
    }


}
