using CSRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DistributedLock4Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Redis distributed lock support
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configure">Configuration delegate</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedLock4Redis(this IServiceCollection services, Action<DistributedLock4RedisOptions> configure)
        {
            var options = new DistributedLock4RedisOptions();
            configure?.Invoke(options);
            services.Configure(configure);
            RedisHelper.Initialization(new CSRedisClient(options.ConnectionString));
            services.AddTransient(typeof(IDistributedLock), typeof(RedisDistributedLock));
            services.AddTransient<LockKeyResolver>();
            return services;
        }

        /// <summary>
        /// Adds Redis distributed lock support
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration interface</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedLock4Redis(this IServiceCollection services, IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("DistributedLock4Redis");
            var options = new DistributedLock4RedisOptions();
            configurationSection.Bind(options);
            services.Configure<DistributedLock4RedisOptions>(configurationSection);
            RedisHelper.Initialization(new CSRedisClient(options.ConnectionString));
            services.AddTransient(typeof(IDistributedLock), typeof(RedisDistributedLock)); 
            services.AddTransient<LockKeyResolver>();
            return services;
        }
    }
}