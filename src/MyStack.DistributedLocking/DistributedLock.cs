using CSRedis;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLocking
{

    public class DistributedLock : IDistributedLock
    {
        private readonly string _keyPrefix;
        public DistributedLock(IOptions<DistributedLockOptions> options)
        {
            _keyPrefix = options?.Value.KeyPrefix;
        }
        public async Task<bool> TryEnterAsync(string key, int expire = 10, int attempt = 60, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "The key cannot be empty or null");
            }

            DateTime begin = DateTime.Now;
            while (true)
            {
                if (await RedisHelper.SetAsync(GetFullKey(key), Thread.CurrentThread.ManagedThreadId, expire, RedisExistence.Nx))
                {
                    return true;
                }
                if (attempt == 0)
                {
                    break;
                }
                if ((DateTime.Now - begin).TotalSeconds >= attempt)
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
        public async Task<T> TryEnterAsync<T>(string key, Func<Task<T>> handle, int expire = 10, int attempt = 60, CancellationToken cancellation = default)
        {
            cancellation.ThrowIfCancellationRequested();
            if (await TryEnterAsync(key, expire, attempt))
            {
                try
                {
                    return await handle?.Invoke();
                }
                finally
                {
                    try { await ExitAsync(key); } finally { }
                }
            }
            return await Task.FromResult(default(T));
        }

    }
}
