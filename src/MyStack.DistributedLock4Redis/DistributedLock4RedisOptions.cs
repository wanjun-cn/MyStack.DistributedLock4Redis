namespace Microsoft.Extensions.DistributedLock4Redis
{
    /// <summary>
    /// Represents a distributed lock configuration item
    /// </summary>
    public class DistributedLock4RedisOptions
    {
        /// <summary>
        /// Get or set the prefix name of the key 
        /// </summary>
        public string KeyPrefix { get; set; } = default!;
        /// <summary>
        /// Get or set Redis connection string 
        /// </summary>
        public string ConnectionString { get; set; } = default!;
    }
}