using CSRedis;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{

    public class RedisDistributedLock : IDistributedLock
    {
        private readonly string? _keyPrefix;
        public RedisDistributedLock(IOptions<DistributedLock4RedisOptions> options)
        {
            _keyPrefix = options?.Value.KeyPrefix;
        }
        public async Task<bool> TryAcquireAsync(string key, int? expire, int? attempt, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "The key cannot be empty or null");
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
        public async Task ExitAsync(string key, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            await RedisHelper.DelAsync(GetFullKey(key));
        }
        private string GetFullKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "The key cannot be empty or null");
            return string.IsNullOrEmpty(_keyPrefix) ? key : $"{_keyPrefix}.{key}";
        }
        public async Task<T> TryExecuteAsync<T>(string key, Func<Task<T>> handler, int? expire = null, int? attempt = null, CancellationToken cancellation = default)
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
                    try { await ExitAsync(key); } finally { }
                }
            }
            return default!;
        }
        public async Task TryExecuteAsync(string key, Func<Task> handler, int? expire = null, int? attempt = null, CancellationToken cancellation = default)
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
                    try { await ExitAsync(key); } finally { }
                }
            }
        }

    }
}
