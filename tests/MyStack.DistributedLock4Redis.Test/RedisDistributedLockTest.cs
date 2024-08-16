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
            var result = await distributedLock.TryAcquireAsync("Foo", expire: 60);
            Assert.True(result);
        }
        [Test]
        public async Task TrExecute()
        {
            var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
            var result = await distributedLock.TryExecuteAsync("Foo", () =>
                {
                    return Task.FromResult(1);
                }, 10, 10, CancellationToken.None);
            Assert.That(result, Is.EqualTo(1));
        }
    }
}
