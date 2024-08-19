using CSRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DistributedLock4Redis;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    { 
        /// <summary>
        /// 添加Redis分布式锁支持
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configure">配置委托</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedLock4Redis(this IServiceCollection services, Action<DistributedLock4RedisOptions> configure)
        {
            var options = new DistributedLock4RedisOptions();
            configure?.Invoke(options);
            services.Configure(configure);
            RedisHelper.Initialization(new CSRedisClient(options.ConnectionString));
            services.AddSingleton(typeof(IDistributedLock), typeof(RedisDistributedLock));
            return services;
        }
        /// <summary>
        /// 添加Redis分布式锁支持
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置接口</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedLock4Redis(this IServiceCollection services, IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("DistributedLock4Redis");
            var options = new DistributedLock4RedisOptions();
            configurationSection.Bind(options);
            services.Configure<DistributedLock4RedisOptions>(configurationSection);
            RedisHelper.Initialization(new CSRedisClient(options.ConnectionString));
            services.AddSingleton(typeof(IDistributedLock), typeof(RedisDistributedLock));
            return services;
        }
    }
}

