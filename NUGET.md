# MyStack.DistributedLock4Redis

Open-source Lightweight Distributed Lock Library (Based on Redis)
 

# Getting Started

## Add Service Support

```
services.AddDistributedLock4Redis(configure =>
{
   configure.KeyPrefix = "MyStack";
   configure.ConnectionString = "127.0.0.1:6379,password=123456";
});
```

## Using Distributed Locks

```
// Acquire Lock
using (var handle = await distributedLock.TryAcquireAsync("Foo"))
{
    if (handle != null)
    {
        // Write your logical code
    }
}
```

# License 
MIT