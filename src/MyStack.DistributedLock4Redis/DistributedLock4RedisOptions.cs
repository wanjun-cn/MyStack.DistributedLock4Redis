namespace Microsoft.Extensions.DistributedLock4Redis
{
    /// <summary>
    /// 表示分布式锁的配置项
    /// </summary>
    public class DistributedLock4RedisOptions
    {
        /// <summary>
        /// 获取或设置全局键的前缀
        /// </summary>
        public string KeyPrefix { get; set; } = default!;
        /// <summary>
        /// 获取或设置Redis连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = default!;
    }
}