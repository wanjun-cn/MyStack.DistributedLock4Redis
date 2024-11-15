using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedLock4Redis;

namespace MyStack.DistributedLock4Redis.Test
{
    public class RedisDistributedLockTest : TestBase
    {
        [Test]
        public async Task TryAcquire()
        {
            var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
            using (var handle = await distributedLock.TryAcquireAsync("Foo"))
            {
                if (handle != null)
                {
                    // Write your logical code
                }
            }
        }
    }
}
