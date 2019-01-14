using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DotEasy.Rpc.Core.Cache.Caching;
using DotEasy.Rpc.Core.Cache.HashAlgorithms;
using DotEasy.Rpc.Core.Cache.Model;

namespace DotEasy.Rpc.Core.Cache.Impl.Redis
{
    /// <summary>
    /// redis数据上下文
    /// </summary>
    public class DefaultRedisContext
    {
        private readonly IHashAlgorithm _hashAlgorithm;

        /// <summary>
        /// 缓存对象集合容器池
        /// </summary>
        internal Lazy<Dictionary<string, List<string>>> _cachingContextPool;

        /// <summary>
        /// 密码
        /// </summary>
        internal string _password = null;

        /// <summary>
        /// 默认缓存失效时间
        /// </summary>
        internal string _defaultExpireTime = null;

        /// <summary>
        /// 连接失效时间
        /// </summary>
        internal string _connectTimeout = null;

        /// <summary>
        /// 哈希节点容器
        /// </summary>
        internal ConcurrentDictionary<string, ConsistentHash<ConsistentHashNode>> dicHash;

        /// <summary>
        /// 对象池上限
        /// </summary>
        internal string _maxSize = null;

        /// <summary>
        /// 对象池下限
        /// </summary>
        internal string _minSize = null;

        /// <summary>
        /// redis数据上下文
        /// </summary>
        /// <param name="args">参数</param>
        public DefaultRedisContext(params object[] args)
        {
            if (CachingContainer.IsRegistered<IHashAlgorithm>())
                _hashAlgorithm = CachingContainer.GetService<IHashAlgorithm>();
            else
                _hashAlgorithm = CachingContainer.GetInstances<IHashAlgorithm>();
            foreach (var arg in args)
            {
                var properties = arg.GetType().GetProperties();
                var field = GetType()
                    .GetField(string.Format("_{0}", properties[0].GetValue(arg)),
                        BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (properties.Count() == 3)
                {
                    _cachingContextPool = new Lazy<Dictionary<string, List<string>>>(
                        () =>
                        {
                            var dataContextPool = new Dictionary<string, List<string>>();
                            var lArg = arg as List<object>;
                            Debug.Assert(lArg != null, nameof(lArg) + " != null");
                            foreach (var tmpArg in lArg)
                            {
                                var props = tmpArg.GetType().GetTypeInfo().GetProperties();
                                var items = props[2].GetValue(tmpArg) as object[];
                                if (items != null)
                                {
                                    var list = (from item in items
                                        let itemProperties = item.GetType().GetProperties()
                                        select itemProperties[1].GetValue(item)
                                        into value
                                        select value.ToString()).ToList();
                                    dataContextPool.Add(props[1].GetValue(tmpArg).ToString(), list);
                                }
                            }

                            return dataContextPool;
                        }
                    );
                }
                else
                {
                    if (field != null) field.SetValue(this, properties[1].GetValue(arg));
                }
            }

            dicHash = new ConcurrentDictionary<string, ConsistentHash<ConsistentHashNode>>();
            InitSettingHashStorage();
        }


        public string ConnectTimeout => _connectTimeout;

        public string DefaultExpireTime => _defaultExpireTime;

        /// <summary>
        /// 缓存对象集合容器池
        /// </summary>
        public Dictionary<string, List<string>> DataContextPool => _cachingContextPool.Value;


        /// <summary>
        /// 初始化设置哈希节点容器
        /// </summary>
        private void InitSettingHashStorage()
        {
            foreach (var dataContext in DataContextPool)
            {
                CacheTargetType targetType;
                if (!Enum.TryParse(dataContext.Key, true, out targetType)) continue;
                var hash =
                    new ConsistentHash<ConsistentHashNode>(_hashAlgorithm);

                dataContext.Value.ForEach(v =>
                {
                    var db = "";
                    var dbs = v.Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
                    var server = v.Split('@');
                    var endpoints = server.Length > 1 ? server[1].Split(':') : server[0].Split(':');
                    var account = server.Length > 1 ? server[0].Split(':') : null;
                    var username = account != null && account.Length > 1 ? account[0] : null;
                    Debug.Assert(account != null, nameof(account) + " != null");
                    var password = server.Length > 1 ? account[account.Length - 1] : _password;
                    if (endpoints.Length <= 1) return;
                    if (dbs.Length > 1)
                    {
                        db = dbs[dbs.Length - 1];
                    }

                    var node = new ConsistentHashNode
                    {
                        Type = targetType,
                        Host = endpoints[0],
                        Port = endpoints[1],
                        UserName = username,
                        Password = password,
                        MaxSize = _maxSize,
                        MinSize = _minSize,
                        Db = db.ToString()
                    };
                    hash.Add(node, string.Format("{0}:{1}", node.Host, node.Port));
                    dicHash.GetOrAdd(targetType.ToString(), hash);
                });
            }
        }
    }
}