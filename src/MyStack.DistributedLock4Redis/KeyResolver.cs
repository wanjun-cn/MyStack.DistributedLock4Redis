using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DistributedLock4Redis
{
    public class KeyResolver
    {
        public KeyResolver(IOptions<DistributedLock4RedisOptions> options)
        {
            KeyPrefix = options.Value.KeyPrefix;
        }
        protected string KeyPrefix { get; }
        public virtual string GetKey(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "Key name cannot be null or empty");
            return string.IsNullOrEmpty(KeyPrefix) ? key : $"{KeyPrefix}{key}";
        }
    }
}
