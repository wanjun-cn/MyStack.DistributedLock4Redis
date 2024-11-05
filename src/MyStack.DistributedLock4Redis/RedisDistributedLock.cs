using CSRedis;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{

    public class RedisDistributedLock : IDistributedLock
    {
        public RedisDistributedLock(IOptions<DistributedLock4RedisOptions> options)
        {
            Options = options.Value;
        }
        protected DistributedLock4RedisOptions Options { get; }
        public async virtual Task<bool> TryAcquireAsync(string key, int? expire, int? attempt, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key name cannot be null or empty");
            }

            DateTime begin = DateTime.Now;
            var attemptSeconds = attempt ?? 60;
            while (true)
            {
                if (await RedisHelper.SetAsync(GetFullKey(key), Thread.CurrentThread.ManagedThreadId, expire ?? 10, RedisExistence.Nx))
                {
                    return true;
                }
                if (attemptSeconds == 0)
                {
                    break;
                }
                if ((DateTime.Now - begin).TotalSeconds >= attemptSeconds)
                {
                    break;
                }

                Thread.Sleep(100);
            }
            return false;
        }
        public async virtual Task ReleaseAsync(string key, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            await RedisHelper.DelAsync(GetFullKey(key));
        }
        protected virtual string GetFullKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Key name cannot be null or empty");
            return string.IsNullOrEmpty(Options.KeyPrefix) ? key : $"{Options.KeyPrefix}.{key}";
        }
        public async virtual Task<T> TryExecuteAsync<T>(string key, Func<Task<T>> handler, int? expire = null, int? attempt = null, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            if (await TryAcquireAsync(key, expire, attempt))
            {
                try
                {
                    if (handler != null)
                        return await handler.Invoke();
                }
                finally
                {
                    try { await ReleaseAsync(key); } finally { }
                }
            }
            return default!;
        }
        public async virtual Task TryExecuteAsync(string key, Func<Task> handler, int? expire = null, int? attempt = null, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            if (await TryAcquireAsync(key, expire, attempt))
            {
                try
                {
                    if (handler != null)
                        await handler.Invoke();
                }
                finally
                {
                    try { await ReleaseAsync(key); } finally { }
                }
            }
        }

    }
}
