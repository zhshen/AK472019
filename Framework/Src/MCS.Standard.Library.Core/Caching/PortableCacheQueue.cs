using MCS.Standard.Library.Core.Extensions;
using MCS.Standard.Library.Core.Properties;
using MCS.Standard.Library.Core.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCS.Standard.Library.Core.Caching
{
    /// <summary>
    /// 为一泛型Cache类，和CacheQueue不同的是，PortableCacheQueue内部没有实现LRU算法，
    /// 而且容量大小也不做限制。用户在使用此Cache时同样需要从此类派生一新类，并手工注册，
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PortableCacheQueue<TKey, TValue> : CacheQueueBase, IScavenge
    {
        /// <summary>
        /// Cache项不存在时的委托定义
        /// </summary>
        /// <param name="cache">Cache对列</param>
        /// <param name="key">键值</param>
        /// <returns>新的Cache项</returns>
        public delegate TValue PortableCacheItemNotExistsAction(PortableCacheQueue<TKey, TValue> cache, TKey key);

        private readonly Dictionary<TKey, CacheItem<TKey, TValue>> _innerDictionary = new Dictionary<TKey, CacheItem<TKey, TValue>>();

        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly bool _overrideExistsItem = true;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 构造方法
        /// </summary>
        protected PortableCacheQueue()
            : base()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="overrideExists">Add Cache项时，是否覆盖已有的数据</param>
        protected PortableCacheQueue(bool overrideExists)
        {
            this._overrideExistsItem = overrideExists;
        }

        /// <summary>
        /// 向CacheQueue中增加一Cache项值对，如果相应的key已经存在，则抛出异常
        /// 此种构造方法无相关Dependency，所以此增加Cache项不会过期，只可能当CacheQueue
        /// 的长度超过预先设定时，才可能被清理掉
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <returns>值</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CahceItem项" />
        /// </remarks>
        public TValue Add(TKey key, TValue data)
        {
            this.Add(key, data, null);

            return data;
        }

        /// <summary>
        /// 向CacheQueue中增加一个有关联Dependency的Cache项，如果相应的key已经存在，则抛出异常
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="data">值</param>
        /// <param name="dependency">依赖：相对时间依赖、绝对时间依赖、文件依赖或混合依赖</param>
        /// <returns>值</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CahceItem项" />
        /// </remarks>
        public TValue Add(TKey key, TValue data, DependencyBase dependency)
        {
            key = ConvertCacheKey(key);

            return this._rwLock.DoWriteFunc(() =>
            {
                //删除已经存在而且过期的Cache项
                if (this._innerDictionary.ContainsKey(key) &&
                    ((CacheItem<TKey, TValue>)this._innerDictionary[key]).Dependency != null &&
                    ((CacheItem<TKey, TValue>)this._innerDictionary[key]).Dependency.HasChanged)
                    this.Remove(key);

                CacheItem<TKey, TValue> item = new CacheItem<TKey, TValue>(key, data, dependency, this);

                if (dependency != null)
                {
                    dependency.UtcLastModified = DateTime.UtcNow;
                    dependency.UtcLastAccessTime = DateTime.UtcNow;
                }

                if (this._overrideExistsItem)
                    this._innerDictionary[key] = item;
                else
                    this._innerDictionary.Add(key, item);

#if NetFramework
                this.Counters.EntriesCounter.RawValue = this.innerDictionary.Count;
#endif

                return data;
            });
        }

        /// <summary>
        /// 属性，获取CacheQueue的最大容量
        /// </summary>
        public override int Count
        {
            get
            {
                return this._innerDictionary.Count;
            }
        }

        /// <summary>
        /// 通过Cache项的key获取Cache项Value的索引器
        /// </summary>
        /// <param name="key">cache项key</param>
        /// <returns>cache项Value</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="通过Cache项的key获取Cache项Value" />
        /// </remarks>
        public TValue this[TKey key]
        {
            get
            {
                key = ConvertCacheKey(key);

#if NetFramework
                this.TotalCounters.HitRatioBaseCounter.Increment();
                this.Counters.HitRatioBaseCounter.Increment();
#endif
                CacheItem<TKey, TValue> item = this._rwLock.DoReadFunc(() => this._innerDictionary[key]);

                this._rwLock.DoWriteAction(() =>
                {
                    this.CheckDependencyChanged(key, item);

                    if (item.Dependency != null)
                        item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                });

#if NetFramework
                this.TotalCounters.HitsCounter.Increment();
                this.TotalCounters.HitRatioCounter.Increment();
                this.Counters.HitsCounter.Increment();
                this.Counters.HitRatioCounter.Increment();
#endif
                return item.Value;
            }
            set
            {
                key = ConvertCacheKey(key);

                this._rwLock.DoWriteAction(() =>
                {
                    CacheItem<TKey, TValue> item = this._innerDictionary[key];

                    item.Value = value;

                    //重置Cache项的最后修改时间和最后访问时间
                    if (item.Dependency != null)
                    {
                        item.Dependency.UtcLastModified = DateTime.UtcNow;
                        item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                    }
                });
            }
        }

        /// <summary>
        /// 判断PortableCacheQueue中是否包含key键的Cache项
        /// </summary>
        /// <param name="key">查询的cache项的键值</param>
        /// <returns>如果包含此键值，返回true，否则返回false</returns>
        public bool ContainsKey(TKey key)
        {
            key = ConvertCacheKey(key);

#if NetFramework
            this.TotalCounters.HitRatioBaseCounter.Increment();
            this.Counters.HitRatioBaseCounter.Increment();
#endif
            return this._rwLock.DoReadFunc(() =>
            {
                bool result = ((this._innerDictionary.ContainsKey(key) &&
                        ((CacheItem<TKey, TValue>)this._innerDictionary[key]).Dependency == null) ||
                        (this._innerDictionary.ContainsKey(key) &&
                        ((CacheItem<TKey, TValue>)this._innerDictionary[key]).Dependency != null &&
                        ((CacheItem<TKey, TValue>)this._innerDictionary[key]).Dependency.HasChanged == false));

#if NetFramework
                if (result)
                {
                    this.TotalCounters.HitsCounter.Increment();
                    this.TotalCounters.HitRatioCounter.Increment();
                    this.Counters.HitsCounter.Increment();
                    this.Counters.HitRatioCounter.Increment();
                }
                else
                {
                    this.TotalCounters.MissesCounter.Increment();
                    this.Counters.MissesCounter.Increment();
                }
#endif
                return result;
            });
        }

        /// <summary>
        /// 通过key，获取Cache项的value，如果相应的cache项存在的话
        /// 则将cache项的value作为输出参数，返回给客户端代码
        /// </summary>
        /// <param name="key">cache项的key</param>
        /// <param name="data">cache项的value</param>
        /// <returns>如果CacheQueue中包含此Cache项，则返回true，否则返回false</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="GetCacheItemTest" lang="cs" title="通过key，获取Cache项的value" />
        /// </remarks>
        public bool TryGetValue(TKey key, out TValue data)
        {
            key = ConvertCacheKey(key);

            return this.InnerTryGetValue(key, out data);
        }

        /// <summary>
        /// 在Cache中读取Cache项，如果不存在，则调用action
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="func">不存在时的回调</param>
        /// <returns>Cache项的值</returns>
        public TValue GetOrAddNewValue(TKey key, Func<PortableCacheQueue<TKey, TValue>, TKey, TValue> func)
        {
            key = ConvertCacheKey(key);

            TValue result = default(TValue);

            if (this.InnerTryGetValue(key, out result) == false)
            {
                result = this._semaphore.DoFunc(() =>
                {
                    TValue innerResult = default(TValue);

                    if (this.InnerTryGetValue(key, out innerResult) == false)
                        innerResult = func(this, key);

                    return innerResult;
                });
            }

            return result;
        }

        public async Task<TValue> GetOrAddNewValueAsync(TKey key, Func<PortableCacheQueue<TKey, TValue>, TKey, Task<TValue>> func)
        {
            key = ConvertCacheKey(key);

            TValue result = default(TValue);

            if (this.InnerTryGetValue(key, out result) == false)
            {
                result = await this._semaphore.DoFuncAsync(async () =>
                {
                    TValue innerResult = default(TValue);

                    if (this.InnerTryGetValue(key, out innerResult) == false)
                        innerResult = await func(this, key);

                    return innerResult;
                });
            }

            return result;
        }

        /// <summary>
        /// 通过key，删除一Cache项
        /// </summary>
        /// <param name="key">键</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CahceItem项" />
        /// </remarks>
        public void Remove(TKey key)
        {
            key = ConvertCacheKey(key);

            this._rwLock.DoWriteAction(() =>
            {
                CacheItem<TKey, TValue> item;

                if (this._innerDictionary.TryGetValue(key, out item))
                    this.InnerRemove(key, item);

#if NetFramework
                this.Counters.EntriesCounter.RawValue = this.innerDictionary.Count;
#endif
            });
        }

        /// <summary>
        /// 重载基类方法，删除传入的CacheItem
        /// </summary>
        /// <param name="cacheItem"></param>
        internal protected override void RemoveItem(CacheItemBase cacheItem)
        {
            this.Remove(((CacheItem<TKey, TValue>)cacheItem).Key);
        }

        /// <summary>
        /// 清空整个CacheQueue，删除CacheQueue中所有的Cache项
        /// </summary>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Caching\PortableCacheQueueTest.cs" region="AddRemoveClearTest" lang="cs" title="增加、移除、获取CahceItem项" />
        /// </remarks>
        public override void Clear()
        {
            this._rwLock.DoWriteAction(() =>
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this._innerDictionary)
                    kp.Value.Dispose();

                this._innerDictionary.Clear();
            });
        }

        /// <summary>
        /// 全部更新了
        /// </summary>
        public override void SetChanged()
        {
            this._rwLock.DoWriteAction(() =>
                this._innerDictionary.ForEach(kp => kp.Value.SetChanged()));
        }

        /// <summary>
        /// 清理方法，清理本CacheQueue中过期的cache项
        /// </summary>
        public void DoScavenging()
        {
            List<KeyValuePair<TKey, CacheItem<TKey, TValue>>> keysToRemove =
                new List<KeyValuePair<TKey, CacheItem<TKey, TValue>>>();

            this._rwLock.DoWriteAction(() =>
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this._innerDictionary)
                    if (kp.Value.Dependency != null && kp.Value.Dependency.HasChanged)
                        keysToRemove.Add(kp);

                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in keysToRemove)
                    this.InnerRemove(kp.Key, kp.Value);
            });
        }

        /// <summary>
        /// 得到所有项的信息
        /// </summary>
        /// <returns></returns>
        public override CacheItemInfoCollection GetAllItemsInfo()
        {
            CacheItemInfoCollection result = new CacheItemInfoCollection();

            this._rwLock.DoReadAction(() =>
            {
                foreach (KeyValuePair<TKey, CacheItem<TKey, TValue>> kp in this._innerDictionary)
                {
                    CacheItemInfo itemInfo = new CacheItemInfo();

                    result.Add(kp.Value.ToCacheItemInfo());
                }
            });

            return result;
        }

        /// <summary>
        /// 转换Cache Key，例如大小写
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual TKey ConvertCacheKey(TKey key)
        {
            return key;
        }

        private bool InnerTryGetValue(TKey key, out TValue data)
        {
            data = default(TValue);
            CacheItem<TKey, TValue> item = null;

#if NetFramework
            this.TotalCounters.HitRatioBaseCounter.Increment();
            this.Counters.HitRatioBaseCounter.Increment();
#endif
            bool result = this._rwLock.DoReadFunc(() =>
                this._innerDictionary.TryGetValue(key, out item)
            );

            TValue outData = data;

            if (result)
            {
                this._rwLock.DoWriteAction(() =>
                {
                    if (this.GetDependencyChanged(key, item))
                        result = false;
                    else
                    {
                        outData = item.Value;
                        if (item.Dependency != null)
                            item.Dependency.UtcLastAccessTime = DateTime.UtcNow;
                    }
                });
            }

            data = outData;

#if NetFramework
            if (result)
            {
                this.TotalCounters.HitsCounter.Increment();
                this.TotalCounters.HitRatioCounter.Increment();
                this.Counters.HitsCounter.Increment();
                this.Counters.HitRatioCounter.Increment();
            }
            else
            {
                this.TotalCounters.MissesCounter.Increment();
                this.Counters.MissesCounter.Increment();
            }
#endif
            return result;
        }

        /// <summary>
        /// 删除Cache项
        /// </summary>
        /// <param name="key">Cache项键值</param>
        /// <param name="item">Cache项</param>
        private void InnerRemove(TKey key, CacheItem<TKey, TValue> item)
        {
            this._innerDictionary.Remove(key);
            item.Dispose();
        }

        private bool GetDependencyChanged(TKey key, CacheItem<TKey, TValue> item)
        {
            bool result = false;

            if (item.Dependency != null && item.Dependency.HasChanged)
            {
                result = true;
                this.InnerRemove(key, item);
            }

            return result;
        }

        /// <summary>
        /// 判断一Cache项是否过期
        /// </summary>
        /// <param name="key">Cache项的键值</param>
        /// <param name="item">Cache项</param>
        /// <returns>如果Cache项过期，返回true，并将其删除，否则返回false</returns>
        private void CheckDependencyChanged(TKey key, CacheItem<TKey, TValue> item)
        {
            if (GetDependencyChanged(key, item))
                throw new DependencyChangedException(string.Format(Resource.DependencyChanged, key, item.Dependency));
        }
    }
}
