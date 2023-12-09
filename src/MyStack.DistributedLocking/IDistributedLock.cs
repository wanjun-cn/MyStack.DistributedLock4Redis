using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLocking
{
    /// <summary>
    /// Represents a distributed lock interface
    /// </summary>
    public interface IDistributedLock
    {
        /// <summary>
        /// Attempts to acquire the exclusive lock on the specified key
        /// </summary>
        /// <param name="key">the name of the key</param>
        /// <param name="expire">Indicates an expiration time (default to 10 seconds)</param>
        /// <param name="attempt">Indicates the attempt time to acquire the lock (default to 60 seconds)</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns> Indicates whether the lock was successfully acquired</returns>
        Task<bool> TryEnterAsync(string key, int expire, int attempt = 60, CancellationToken cancellation = default);

      
        /// <summary>
        /// Releases the exclusive lock on the specified key
        /// </summary>
        /// <param name="key">the name of the key</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns>Indicates whether the lock was successfully released</returns>
        Task ExitAsync(string key, CancellationToken cancellation = default);

        /// <summary>
        /// Attempts to acquire the exclusive lock on the specified key and performs a given operation
        /// </summary>
        /// <param name="key">the name of the key</param>
        /// <param name="handler">Indicate the thing to be handled</param>
        /// <param name="expire">Indicates an expiration time (default to 10 seconds)</param>
        /// <param name="attempt">Indicates the attempt time to acquire the lock (default to 60 seconds)</param>
        /// <param name="cancellation">cancellation token</param>
        /// <returns>Indicates whether the lock was successfully acquired and the operation was completed</returns>
        Task<T> TryEnterAsync<T>(string key, Func<Task<T>> handler, int expire = 10, int attempt = 60, CancellationToken cancellation = default);
    }

}