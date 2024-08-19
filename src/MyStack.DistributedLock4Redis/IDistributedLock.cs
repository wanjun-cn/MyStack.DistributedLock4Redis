using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis
{
    /// <summary>
    /// 表示分布式锁接口
    /// </summary>
    public interface IDistributedLock
    {
        /// <summary>
        /// 尝试获取锁
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="expire">锁的过期时间（默认10s）</param>
        /// <param name="attempt">尝试获取锁的超时时间</param>
        /// <param name="cancellation">取消令牌</param>
        /// <returns>当获取锁成功时返回true，否则返回false</returns>
        Task<bool> TryAcquireAsync(string key, int? expire = null, int? attempt = null, CancellationToken cancellation = default);


        /// <summary>
        /// 释放已获取的锁
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="cancellation">取消令牌</param> 
        Task ReleaseAsync(string key, CancellationToken cancellation = default);
        /// <summary>
        /// 尝试在获取锁成功执行委托任务
        /// </summary>
        /// <typeparam name="T">定义委托任务返回值的类型</typeparam>
        /// <param name="key">键名</param>
        /// <param name="handler">委托任务</param>
        /// <param name="expire">锁的过期时间（默认10s）</param>
        /// <param name="attempt">尝试获取锁的超时时间</param>
        /// <param name="cancellation">取消令牌</param>
        /// <returns>当获取锁成功时返回委托任务返回值，否则返回异常</returns>
        Task<T> TryExecuteAsync<T>(string key, Func<Task<T>> handler, int? expire = null, int? attempt = null, CancellationToken cancellation = default);
        /// <summary>
        /// 尝试在获取锁成功执行委托任务
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="handler">委托任务</param>
        /// <param name="expire">锁的过期时间（默认10s）</param>
        /// <param name="attempt">尝试获取锁的超时时间</param>
        /// <param name="cancellation">取消令牌</param>
        /// <returns></returns>
        Task TryExecuteAsync(string key, Func<Task> handler, int? expire = null, int? attempt = null, CancellationToken cancellation = default);
    }

}