namespace Microsoft.Extensions.DistributedLocking
{
    /// <summary>
    /// Represents a distributed lock configuration item
    /// </summary>
    public class DistributedLockOptions
    {
        /// <summary>
        /// Get or set the prefix name of the key 
        /// </summary>
        public string KeyPrefix { get; set; }
        /// <summary>
        /// Get or set Redis connection string 
        /// </summary>
        public string ConnectionString { get; set; }
    }
}