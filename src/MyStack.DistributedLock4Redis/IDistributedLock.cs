using System;
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
        /// Tries to acquire the lock
        /// </summary>
        /// <param name="key">The key name</param>
        /// <param name="expire">The expiration time of the lock (default 10 seconds)</param>
        /// <param name="attempt">The timeout for attempting to acquire the lock</param>
        /// <param name="cancellation">The cancellation token</param>
        /// <returns>Returns true if the lock is acquired successfully, otherwise false</returns>
        Task<bool> TryAcquireAsync(string key, int? expire = null, int? attempt = null, CancellationToken cancellation = default);

        /// <summary>
        /// Releases the acquired lock
        /// </summary>
        /// <param name="key">The key name</param>
        /// <param name="cancellation">The cancellation token</param>
        Task ReleaseAsync(string key, CancellationToken cancellation = default);

        /// <summary>
        /// Tries to execute the delegate task if the lock is acquired successfully
        /// </summary>
        /// <typeparam name="T">The type of the return value of the delegate task</typeparam>
        /// <param name="key">The key name</param>
        /// <param name="handler">The delegate task</param>
        /// <param name="expire">The expiration time of the lock (default 10 seconds)</param>
        /// <param name="attempt">The timeout for attempting to acquire the lock</param>
        /// <param name="cancellation">The cancellation token</param>
        /// <returns>Returns the result of the delegate task if the lock is acquired successfully, otherwise throws an exception</returns>
        Task<T> TryExecuteAsync<T>(string key, Func<Task<T>> handler, int? expire = null, int? attempt = null, CancellationToken cancellation = default);

        /// <summary>
        /// Tries to execute the delegate task if the lock is acquired successfully
        /// </summary>
        /// <param name="key">The key name</param>
        /// <param name="handler">The delegate task</param>
        /// <param name="expire">The expiration time of the lock (default 10 seconds)</param>
        /// <param name="attempt">The timeout for attempting to acquire the lock</param>
        /// <param name="cancellation">The cancellation token</param>
        Task TryExecuteAsync(string key, Func<Task> handler, int? expire = null, int? attempt = null, CancellationToken cancellation = default);
    }
}