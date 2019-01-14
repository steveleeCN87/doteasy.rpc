using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DotEasy.Rpc.Core.Cache.Caching;

namespace DotEasy.Rpc.Core.Cache.Impl.Memeory
{
    public class DefaultMemoryCache
    {
        private static readonly ConcurrentDictionary<string, Tuple<string, object, DateTime>> Cache =
            new ConcurrentDictionary<string, Tuple<string, object, DateTime>>();

        private const int TaskInterval = 5;

        static DefaultMemoryCache()
        {
            try
            {
                DefaultGCThreadProvider.AddThread(Collect);
            }
            catch (Exception err)
            {
                throw new CachingException(err.Message, err);
            }
        }

        public static int Count => Cache.Count;

        /// <summary>
        /// 获得一个Cache对象
        /// </summary>
        /// <param name="key">标识</param>
        public static object Get(string key)
        {
            Check.CheckCondition(() => string.IsNullOrEmpty(key), "key");
            object result;
            if (Contains(key, out result))
            {
                return result;
            }

            return null;
        }

        public static IDictionary<string, T> Get<T>(IEnumerable<string> keys)
        {
            if (keys == null)
            {
                return new Dictionary<string, T>();
            }

            var dictionary = new Dictionary<string, T>();
            // ReSharper disable once GenericEnumeratorNotDisposed
            IEnumerator<string> enumerator = keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                object obj2 = Get(current);
                if (obj2 is T)
                {
                    dictionary.Add(current ?? throw new Exception("current is empty"), (T) obj2);
                }
            }

            return dictionary;
        }

        public static bool GetCacheTryParse(string key, out object obj)
        {
            Check.CheckCondition(() => string.IsNullOrEmpty(key), "key");
            obj = Get(key);
            return obj != null;
        }

        public static T Get<T>(string key)
        {
            Check.CheckCondition(() => string.IsNullOrEmpty(key), "key");
            object obj2 = Get(key);
            if (obj2 is T)
            {
                return (T) obj2;
            }

            return default(T);
        }

        /// <summary>
        /// 是否存在缓存
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Contains(string key, out object value)
        {
            bool isSuccess = false;
            Tuple<string, object, DateTime> item;
            value = null;
            if (Cache.TryGetValue(key, out item))
            {
                value = item.Item2;
                isSuccess = item.Item3 > DateTime.Now;
            }

            return isSuccess;
        }

        public static void Set(string key, object value, double cacheSecond)
        {
            DateTime cacheTime = DateTime.Now.AddSeconds(cacheSecond);
            var cacheValue = new Tuple<string, object, DateTime>(key, value, cacheTime);
            Cache.AddOrUpdate(key, cacheValue, (v, oldValue) => cacheValue);
        }

        public static void RemoveByPattern(string pattern)
        {
            // ReSharper disable once GenericEnumeratorNotDisposed
            var enumerator = Cache.GetEnumerator();
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            while (enumerator.MoveNext())
            {
                var input = enumerator.Current.Key;
                if (regex.IsMatch(input))
                {
                    Remove(input);
                }
            }
        }

        public static void Remove(string key)
        {
            Cache.TryRemove(key, out _);
        }

        public static void Dispose()
        {
            Cache.Clear();
        }

        private static void Collect(object threadId)
        {
            while (true)
            {
                try
                {
                    var cacheValues = Cache.Values;
                    cacheValues = cacheValues.OrderBy(p => p.Item3).ToList();
                    foreach (var cacheValue in cacheValues)
                    {
                        if ((cacheValue.Item3 - DateTime.Now).Seconds <= 0)
                        {
                            Cache.TryRemove(cacheValue.Item1, out _);
                        }
                    }

                    Thread.Sleep(TaskInterval * 60 * 1000);
                }
                catch
                {
                    Dispose();
                    DefaultGCThreadProvider.AddThread(Collect);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}