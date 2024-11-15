using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{
    /// <summary>
    /// Represents a distributed lock interface
    /// </summary>
    public interface IDistributedLock
    {
        /// <summary>
        /// Tries to acquire the lock handle
        /// </summary>
        /// <param name="key">The key name</param>
        /// <param name="expireSeconds">The expiration time of the lock (default 10 seconds)</param>
        /// <param name="attemptSeconds">The timeout for attempting to acquire the lock</param>
        /// <param name="cancellation">The cancellation token</param>
        /// <returns>Returns IDistributedLockHandle if the lock is acquired successfully, otherwise null</returns>
        Task<IDistributedLockHandle?> TryAcquireAsync(string key, int? expireSeconds = null, int? attemptSeconds = null, CancellationToken cancellation = default);
    }
}