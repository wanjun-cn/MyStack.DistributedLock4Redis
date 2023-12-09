using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DistributedLocking;

namespace MyStack.DistributedLocking.Test
{
    public class DistractedLockTest : TestBase
    {
        [Test]
        public async Task TryEnter()
        {
            var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
            var result = await distributedLock.TryEnterAsync("Foo", expire: 60);
            Assert.True(result);
        }
        [Test]
        public async Task TryEnter_With_Handler()
        {
            var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
            var result = await distributedLock.TryEnterAsync("Foo", async () =>
            {
                return await Task.FromResult(1);
            });
            Assert.That(result, Is.EqualTo(1));
        }
    }
}
