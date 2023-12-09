using CSRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DistributedLocking;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    { 
        public static IServiceCollection AddDistributedLocking(this IServiceCollection services, Action<DistributedLockOptions> configure)
        {
            var options = new DistributedLockOptions();
            configure?.Invoke(options);
            services.Configure(configure);
            RedisHelper.Initialization(new CSRedisClient(options.ConnectionString));
            services.AddSingleton(typeof(IDistributedLock), typeof(DistributedLock));
            return services;
        } 
        public static IServiceCollection AddDistributedLocking(this IServiceCollection services, IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("DistributedLocking");
            var options = new DistributedLockOptions();
            configurationSection.Bind(options);
            services.Configure<DistributedLockOptions>(configurationSection);
            RedisHelper.Initialization(new CSRedisClient(options.ConnectionString));
            services.AddSingleton(typeof(IDistributedLock), typeof(DistributedLock));
            return services;
        }
    }
}

