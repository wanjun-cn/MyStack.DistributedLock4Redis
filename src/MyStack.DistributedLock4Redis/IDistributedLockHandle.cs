using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{
    public interface IDistributedLockHandle : IDisposable
    {
        Task DisposeAsync();
    }
}
