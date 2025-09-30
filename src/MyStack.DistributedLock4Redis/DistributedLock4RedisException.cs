using System;

namespace Microsoft.Extensions.DistributedLock4Redis
{
    public class DistributedLock4RedisException : Exception
    {
        public DistributedLock4RedisException(string message) : base(message)
        {
        }
        public DistributedLock4RedisException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
