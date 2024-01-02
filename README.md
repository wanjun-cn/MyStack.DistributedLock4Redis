# MyStack.DistributedLocking

Open source lightweight distributed lock library


| nuget      | stats |
| ----------- | ----------- |
| [![nuget](https://img.shields.io/nuget/v/MyStack.DistributedLocking.svg?style=flat-square)](https://www.nuget.org/packages/MyStack.DistributedLocking)       |  [![stats](https://img.shields.io/nuget/dt/MyStack.DistributedLocking.svg?style=flat-square)](https://www.nuget.org/stats/packages/MyStack.DistributedLocking?groupby=Version)        |

# Usage

## Add Services to container

```
services.AddDistributedLocks(configure =>
{
   configure.KeyPrefix = "MyStack";
   configure.ConnectionString = "127.0.0.1:6379,password=123456";
});
```

## Use DistributedLock

```
// lock a key
var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
var result = await distributedLock.TryEnterAsync("Foo", expire: 60);
// release lock
await distributedLock.ExitAsync("Foo");


or 

// Lock a key and release it after executing a method
var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
var result = await distributedLock.TryEnterAsync("Foo", async () =>
{
    return await Task.FromResult(1);
});
```

# License 
MIT