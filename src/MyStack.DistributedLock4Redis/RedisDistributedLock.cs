using Microsoft.Extensions.DistributedLock4Redis.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{
    public class RedisDistributedLock : IDistributedLock
    {
        private readonly KeyResolver _lockKeyResolver;
        private readonly DistributedLock4RedisOptions _options;
        public RedisDistributedLock(
            IOptions<DistributedLock4RedisOptions> optionsAccessor,
            KeyResolver lockKeyResolver)
        {
            _options = optionsAccessor.Value;
            _lockKeyResolver = lockKeyResolver;
        }
        public async Task<IDistributedLockHandle?> TryAcquireAsync(string key, int? expireSeconds = null, int? attemptSeconds = null, CancellationToken cancellation = default)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "Key name cannot be null or empty");

            key = _lockKeyResolver.GetKey(key);

            DateTime now = DateTime.Now;
            while (!cancellation.IsCancellationRequested)
            {
                if (await RedisClient.SetAsync(key, Thread.CurrentThread.ManagedThreadId.ToString(), expireSeconds ?? _options.DefaultExpireSeconds, true))
                {
                    return new RedisDistributedLockHandle(key);
                }
                if (attemptSeconds == 0)
                {
                    break;
                }
                if ((DateTime.Now - now).TotalSeconds >= (attemptSeconds ?? _options.DefaultAttemptSeconds))
                {
                    break;
                }
                await Task.Delay(100);
            }
            return null;
        }
    }
}
