using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyStack.DistributedLock4Redis.Test
{
    public abstract class TestBase
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        [SetUp]
        public void Setup()
        {
            var builder = new HostBuilder()
              .ConfigureHostConfiguration(configure =>
              {
                  configure.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json");
              })
              .ConfigureServices((context, services) =>
              {
                  services.AddDistributedLock4Redis(context.Configuration);
                  // or  
                  //services.AddDistributedLocks(configure =>
                  //{
                  //    configure.KeyPrefix = context.Configuration["DistributedLocks:KeyPrefix"];
                  //    configure.ConnectionString = context.Configuration["DistributedLocks:ConnectionString"];
                  //});
              });

            var app = builder.Build();
            ServiceProvider = app.Services;
        }
    }
}