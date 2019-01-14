using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Cache.Caching;

namespace DotEasy.Rpc.Core.Cache
{
    public interface ICacheProvider
    {
        /// <summary>
        /// 缓存连接点
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        Task<bool> ConnectionAsync(CachingEndpoint endpoint);

        /// <summary>
        /// 添加一个key 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Add(string key, object value);

        /// <summary>
        /// 异步添加一个key 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddAsync(string key, object value);

        /// <summary>
        /// 添加一个key，默认时限
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="defaultExpire"></param>
        void Add(string key, object value, bool defaultExpire);

        /// <summary>
        /// 异步添加一个key，默认时限
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="defaultExpire"></param>
        void AddAsync(string key, object value, bool defaultExpire);

        /// <summary>
        /// 添加一个key，指定时限
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="numOfMinutes"></param>
        void Add(string key, object value, long numOfMinutes);

        /// <summary>
        /// 异步添加一个key，指定时限
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="numOfMinutes"></param>
        void AddAsync(string key, object value, long numOfMinutes);

        void Add(string key, object value, TimeSpan timeSpan);
        void AddAsync(string key, object value, TimeSpan timeSpan);

        IDictionary<string, T> Get<T>(IEnumerable<string> keys);
        Task<IDictionary<string, T>> GetAsync<T>(IEnumerable<string> keys);
        object Get(string key);
        Task<object> GetAsync(string key);
        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);
        bool GetCacheTryParse(string key, out object obj);
        void Remove(string key);
        void RemoveAsync(string key);

        #region 字段

        long DefaultExpireTime { get; set; }
        string KeySuffix { get; set; }

        #endregion
    }
}