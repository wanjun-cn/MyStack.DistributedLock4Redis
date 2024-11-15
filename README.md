# MyStack.DistributedLock4Redis

Open-source Lightweight Distributed Lock Library (Based on Redis)

| nuget      | stats |
| ----------- | ----------- |
| [![nuget](https://img.shields.io/nuget/v/MyStack.DistributedLock4Redis.svg?style=flat-square)](https://www.nuget.org/packages/MyStack.DistributedLock4Redis)       |  [![stats](https://img.shields.io/nuget/dt/MyStack.DistributedLock4Redis.svg?style=flat-square)](https://www.nuget.org/stats/packages/MyStack.DistributedLock4Redis?groupby=Version)        |

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