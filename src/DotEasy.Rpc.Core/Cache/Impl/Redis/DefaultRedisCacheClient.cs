using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Attributes;
using DotEasy.Rpc.Core.Cache.Caching;
using DotEasy.Rpc.Core.Cache.HashAlgorithms;
using DotEasy.Rpc.Core.Cache.Model;
using StackExchange.Redis;

namespace DotEasy.Rpc.Core.Cache.Impl.Redis
{
    
    [IdentifyCache(CacheTargetType.Redis)]
    public class DefaultRedisCacheClient<T> : ICacheClient<T> where T : class
    {
        private static readonly ConcurrentDictionary<string, Lazy<CachingObjectPool<T>>> Pool = new ConcurrentDictionary<string, Lazy<CachingObjectPool<T>>>();

        // ReSharper disable once EmptyConstructor
        public DefaultRedisCacheClient()
        {
        }

        public async Task<bool> ConnectionAsync(CachingEndpoint endpoint, int connectTimeout)
        {
            ConnectionMultiplexer conn = null;
            try
            {
                var info = endpoint as ConsistentHashNode;
                // ReSharper disable once PossibleNullReferenceException
                var point = string.Format("{0}:{1}", info.Host, info.Port);
                conn = await ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions()
                {
                    EndPoints = {{point}},
                    ServiceName = point,
                    Password = info.Password,
                    ConnectTimeout = connectTimeout
                });
                return conn.IsConnected;
            }
            catch (Exception e)
            {
                throw new CachingException(e.Message);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public T GetClient(CachingEndpoint endpoint, int connectTimeout)
        {
            try
            {
                var info = endpoint as DefaultRedisEndpoint;
                Check.NotNull(info, "endpoint");
                // ReSharper disable once PossibleNullReferenceException
                var key = string.Format("{0}{1}{2}{3}", info.Host, info.Port, info.Password, info.DbIndex);
                if (!Pool.ContainsKey(key))
                {
                    var objectPool = new Lazy<CachingObjectPool<T>>(() => new CachingObjectPool<T>(() =>
                    {
                        var point = string.Format("{0}:{1}", info.Host, info.Port);
                        var redisClient = ConnectionMultiplexer.Connect(new ConfigurationOptions()
                        {
                            EndPoints = {point},
                            ServiceName = point,
                            Password = info.Password,
                            ConnectTimeout = connectTimeout,
                            AbortOnConnectFail = false
                        });
                        return redisClient.GetDatabase(info.DbIndex) as T;
                    }, info.MinSize, info.MaxSize));
                    Pool.GetOrAdd(key, objectPool);
                    return objectPool.Value.GetObject();
                }

                return Pool[key].Value.GetObject();
            }
            catch (Exception e)
            {
                throw new CachingException(e.Message);
            }
        }
    }
}