using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedLock4Redis;
using Microsoft.Extensions.DistributedLock4Redis.Internal;
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
            services.AddTransient(typeof(IDistributedLock), typeof(RedisDistributedLock));
            services.AddSingleton<KeyResolver>();
            RedisClient.Initialize(options.ConnectionString);
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
            services.AddTransient(typeof(IDistributedLock), typeof(RedisDistributedLock));
            services.AddSingleton<KeyResolver>();
            RedisClient.Initialize(options.ConnectionString);
            return services;
        }
    }
}