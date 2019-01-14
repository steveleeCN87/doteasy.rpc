using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Attributes;
using DotEasy.Rpc.Core.Cache.Caching;
using DotEasy.Rpc.Core.Cache.Impl.Redis;
using DotEasy.Rpc.Core.Cache.Model;

namespace DotEasy.Rpc.Core.Cache.Impl.Memeory
{
    [IdentifyCache(CacheTargetType.MemoryCache)]
    public class DefaultMemoryCacheProvider : ICacheProvider
    {
        #region 字段

        /// <summary>
        /// 缓存数据上下文
        /// </summary>
        private readonly Lazy<DefaultRedisContext> _context;

        /// <summary>
        /// 默认失效时间
        /// </summary>
        private Lazy<long> _defaultExpireTime;

        /// <summary>
        /// 配置失效时间
        /// </summary>
        private const double ExpireTime = 60D;

        /// <summary>
        /// KEY键前缀
        /// </summary>
        private string _keySuffix;

        #endregion

        #region 构造函数

        public DefaultMemoryCacheProvider(string appName)
        {
            _context = new Lazy<DefaultRedisContext>(() =>
            {
                return CachingContainer.IsRegistered<DefaultRedisContext>(CacheTargetType.Redis.ToString())
                    ? CachingContainer.GetService<DefaultRedisContext>(appName)
                    : CachingContainer.GetInstances<DefaultRedisContext>(appName);
            });

            _keySuffix = appName;
            _defaultExpireTime = new Lazy<long>(() => long.Parse(_context.Value._defaultExpireTime));
        }

        public DefaultMemoryCacheProvider()
        {
            _defaultExpireTime = new Lazy<long>(() => 60);
            _keySuffix = string.Empty;
        }


        public void Add(string key, object value)
        {
            DefaultMemoryCache.Set(GetKeySuffix(key), value, _defaultExpireTime.Value);
        }

        public async void AddAsync(string key, object value)
        {
            await Task.Run(() => DefaultMemoryCache.Set(GetKeySuffix(key), value, DefaultExpireTime));
        }

        public void Add(string key, object value, bool defaultExpire)
        {
            DefaultMemoryCache.Set(GetKeySuffix(key), value, defaultExpire ? DefaultExpireTime : ExpireTime);
        }

        public async void AddAsync(string key, object value, bool defaultExpire)
        {
            await Task.Run(() => DefaultMemoryCache.Set(GetKeySuffix(key), value, defaultExpire ? DefaultExpireTime : ExpireTime));
        }

        public void Add(string key, object value, long numOfMinutes)
        {
            DefaultMemoryCache.Set(GetKeySuffix(key), value, numOfMinutes);
        }

        public async void AddAsync(string key, object value, long numOfMinutes)
        {
            await Task.Run(() => DefaultMemoryCache.Set(GetKeySuffix(key), value, numOfMinutes));
        }

        public void Add(string key, object value, TimeSpan timeSpan)
        {
            DefaultMemoryCache.Set(GetKeySuffix(key), value, timeSpan.TotalSeconds);
        }

        public async void AddAsync(string key, object value, TimeSpan timeSpan)
        {
            await Task.Run(() => DefaultMemoryCache.Set(GetKeySuffix(key), value, timeSpan.TotalSeconds));
        }

        public IDictionary<string, T> Get<T>(IEnumerable<string> keys)
        {
            keys.ToList().ForEach(key => GetKeySuffix(key));
            return DefaultMemoryCache.Get<T>(keys);
        }

        public async Task<IDictionary<string, T>> GetAsync<T>(IEnumerable<string> keys)
        {
            keys.ToList().ForEach(key => GetKeySuffix(key));
            var result = await Task.Run(() => DefaultMemoryCache.Get<T>(keys));
            return result;
        }

        public object Get(string key)
        {
            return DefaultMemoryCache.Get(GetKeySuffix(key));
        }

        public async Task<object> GetAsync(string key)
        {
            var result = await Task.Run(() => DefaultMemoryCache.Get(GetKeySuffix(key)));
            return result;
        }

        public T Get<T>(string key)
        {
            return DefaultMemoryCache.Get<T>(GetKeySuffix(key));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var result = await Task.Run(() => DefaultMemoryCache.Get<T>(GetKeySuffix(key)));
            return result;
        }

        public bool GetCacheTryParse(string key, out object obj)
        {
            return DefaultMemoryCache.GetCacheTryParse(GetKeySuffix(key), out obj);
        }

        public void Remove(string key)
        {
            DefaultMemoryCache.RemoveByPattern(GetKeySuffix(key));
        }

        public async void RemoveAsync(string key)
        {
            await Task.Run(() => DefaultMemoryCache.Remove(GetKeySuffix(key)));
        }

        public Task<bool> ConnectionAsync(CachingEndpoint endpoint)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 默认缓存失效时间
        /// </summary>
        public long DefaultExpireTime
        {
            get { return _defaultExpireTime.Value; }
            set { _defaultExpireTime = new Lazy<long>(() => value); }
        }

        /// <summary>
        /// KEY前缀
        /// </summary>
        public string KeySuffix
        {
            get { return _keySuffix; }
            set { _keySuffix = value; }
        }

        #endregion

        #region 私有变量

        private string GetKeySuffix(string key)
        {
            return string.IsNullOrEmpty(KeySuffix) ? key : string.Format("_{0}_{1}", KeySuffix, key);
        }

        #endregion
    }
}