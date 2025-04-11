using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{
    public class RedisDistributedLockHandle : IDistributedLockHandle
    {
        public string FullKey { get; }
        public RedisDistributedLockHandle(string fullKey)
        {
            FullKey = fullKey;
        }
        public void Dispose()
        {
            RedisHelper.Del(FullKey);
        }

        public async Task DisposeAsync()
        {
            await RedisHelper.DelAsync(FullKey);
        }
    }
}
