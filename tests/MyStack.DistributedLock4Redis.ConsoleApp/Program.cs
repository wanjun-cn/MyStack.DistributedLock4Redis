using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedLock4Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyStack.DistributedLock4Redis.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(configure =>
                {
                    configure.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(logging =>
                    {
                        logging.AddConsole(c => c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss]");
                    });
                    services.AddDistributedLock4Redis(context.Configuration);
                });

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(Program));
            var distributedLock = app.Services.GetRequiredService<IDistributedLock>();

            var tasks = Enumerable.Range(0, 15).Select(async i =>
            {
                try
                {
                    using (var handle = await distributedLock.TryAcquireAsync("Foo", attemptSeconds: 200))
                    {
                        if (handle != null)
                        {
                            logger.LogInformation($"Task {i} acquired the lock.");
                            // 模拟业务处理
                            await Task.Delay(1000);
                            logger.LogInformation($"Task {i} released the lock.");
                        }
                        else
                        {
                            logger.LogWarning($"Task {i} could not acquire the lock.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Task {i} encountered an error");
                }
            }).ToArray();

            await Task.WhenAll(tasks);
            logger.LogInformation("All 15 tasks completed.");

            app.Run();

        }
    }
}
