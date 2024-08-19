# MyStack.DistributedLock4Redis

开源的轻量级分布式锁类库

| nuget      | stats |
| ----------- | ----------- |
| [![nuget](https://img.shields.io/nuget/v/MyStack.DistributedLock4Redis.svg?style=flat-square)](https://www.nuget.org/packages/MyStack.DistributedLock4Redis)       |  [![stats](https://img.shields.io/nuget/dt/MyStack.DistributedLock4Redis.svg?style=flat-square)](https://www.nuget.org/stats/packages/MyStack.DistributedLock4Redis?groupby=Version)        |

# 开始使用

## 添加服务支持

```
services.AddDistributedLock4Redis(configure =>
{
   configure.KeyPrefix = "MyStack";
   configure.ConnectionString = "127.0.0.1:6379,password=123456";
});
```

## 使用分布式

```
// 获取锁
var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
var result = await distributedLock.TryEnterAsync("Foo", expire: 60);
// 释放锁
await distributedLock.ReleaseAsync("Foo");


or 

// 尝试获取锁并执行异步任务
var distributedLock = ServiceProvider.GetRequiredService<IDistributedLock>();
var result = await distributedLock.TryEnterAsync("Foo", async () =>
{
    return await Task.FromResult(1);
});
```

# 许可 
MIT