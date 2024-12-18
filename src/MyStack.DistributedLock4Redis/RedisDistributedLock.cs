using CSRedis;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{

    public class RedisDistributedLock : IDistributedLock
    {
        protected LockKeyResolver LockKeyResolver { get; }
        public RedisDistributedLock(IOptions<DistributedLock4RedisOptions> options, LockKeyResolver lockKeyResolver)
        {
            Options = options.Value;
            LockKeyResolver = lockKeyResolver;
        }
        protected DistributedLock4RedisOptions Options { get; }
        public async Task<IDistributedLockHandle?> TryAcquireAsync(string key, int? expireSeconds = null, int? attemptSeconds = null, CancellationToken cancellation = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "Key name cannot be null or empty");
            }

            key = LockKeyResolver.GetKey(key);

            DateTime begin = DateTime.Now;
            while (!cancellation.IsCancellationRequested)
            {
                if (await RedisHelper.SetAsync(key, Thread.CurrentThread.ManagedThreadId, attemptSeconds ?? Options.DefaultExpireSeconds, RedisExistence.Nx))
                {
                    return new RedisDistributedLockHandle(key);
                }
                if (attemptSeconds == 0)
                {
                    break;
                }
                if ((DateTime.Now - begin).TotalSeconds >= (expireSeconds ?? Options.DefaultAttemptSeconds))
                {
                    break;
                }
                await Task.Delay(100);
            }
            return null;
        }
    }
}
