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
            using (var handle = await distributedLock.TryAcquireAsync("Foo", expireSeconds: 60, attemptSeconds: 10))
            {
                if (handle != null)
                {
                    // Write your logical code
                    await Task.Delay(15000);
                }
                else
                {
                    throw new InvalidOperationException("AA");
                }
            }

        }
    }
}
