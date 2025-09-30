using Microsoft.Extensions.DistributedLock4Redis.Internal;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{
    internal class RedisDistributedLockHandle : IDistributedLockHandle
    {
        public string Key { get; }
        private bool _disposed = false;
        public RedisDistributedLockHandle(string key)
        {
            Key = key;
        }

        public void Dispose()
        {
            if (_disposed) return;
            try
            {
                RedisClient.DelAsync(Key).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            finally
            {
                _disposed = true;
            }
        }
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            try
            {
                await RedisClient.DelAsync(Key);
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}
