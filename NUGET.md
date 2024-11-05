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
var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
var result = await distributedLock.TryAcquireAsync("Foo", expire: 60);
// Release Lock
await distributedLock.ReleaseAsync("Foo");


or 

// Try to Acquire Lock and Execute Asynchronous Task
var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
var result = await distributedLock.TryExecuteAsync("Foo", async () =>
{
    return await Task.FromResult(1);
});
```

# License 
MIT