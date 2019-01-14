using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using DotEasy.Rpc.Core.Attributes;
using DotEasy.Rpc.Core.Cache.Caching;
using DotEasy.Rpc.Core.Cache.Model;
using DotEasy.Rpc.Core.DependencyResolver;
using DotEasy.Rpc.Core.DependencyResolver.Builder;
using Microsoft.Extensions.Configuration;

namespace DotEasy.Rpc.Core.ConfigCenter
{
    public class CacheConfig
    {
        private const string CacheSectionName = "CachingProvider";
        private readonly CachingProvider _cacheWrapperSetting;
#pragma warning disable 414
        internal static string Path = null;
#pragma warning restore 414
        internal static IConfigurationRoot Configuration { get; set; }

        public CacheConfig()
        {
            RpcServiceResolver.Current.Register(null, Activator.CreateInstance(typeof(HashAlgorithm), new object[] { }));
            _cacheWrapperSetting = Configuration.Get<CachingProvider>();
            RegisterConfigInstance();
            RegisterLocalInstance("ICacheClient`1");
            InitSettingMethod();
        }

        internal static CacheConfig DefaultInstance
        {
            get
            {
                var config = RpcServiceResolver.Current.GetService<CacheConfig>();
                if (config == null)
                {
                    config = Activator.CreateInstance(typeof(CacheConfig), new object[] { }) as CacheConfig;
                    RpcServiceResolver.Current.Register(null, config);
                }

                return config;
            }
        }

        public T GetContextInstance<T>() where T : class
        {
            var context = RpcServiceResolver.Current.GetService<T>(typeof(T));
            return context;
        }

        public T GetContextInstance<T>(string name) where T : class
        {
            var context = RpcServiceResolver.Current.GetService<T>(name);
            return context;
        }

        private void RegisterLocalInstance(string typeName)
        {
            var types = GetType().GetTypeInfo().Assembly.GetTypes().Where(p => p.GetTypeInfo().GetInterface(typeName) != null);
            foreach (var t in types)
            {
                var attribute = t.GetTypeInfo().GetCustomAttribute<IdentifyCacheAttribute>();
                RpcServiceResolver.Current.Register(attribute.Name.ToString(),
                    Activator.CreateInstance(t));
            }
        }

        private void RegisterConfigInstance()
        {
            var bingingSettings = _cacheWrapperSetting.CachingSettings;
            try
            {
                var types = GetType().GetTypeInfo().Assembly.GetTypes().Where(p => p.GetTypeInfo().GetInterface("ICacheProvider") != null);
                foreach (var t in types)
                {
                    foreach (var setting in bingingSettings)
                    {
                        var properties = setting.Properties;
                        var args = properties.Select(p => GetTypedPropertyValue(p)).ToArray();

                        var maps =
                            properties.Select(p => p.Maps)
                                .FirstOrDefault(p => p != null && p.Any());
                        var type = Type.GetType(setting.Class, true);
                        if (RpcServiceResolver.Current.GetService(type, setting.Id) == null)
                            RpcServiceResolver.Current.Register(setting.Id, Activator.CreateInstance(type, args));
                        if (maps == null) continue;
                        if (!maps.Any()) continue;
                        foreach (
                            var mapsetting in
                            maps.Where(mapsetting => t.Name.StartsWith(mapsetting.Name, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            RpcServiceResolver.Current.Register(string.Format("{0}.{1}", setting.Id, mapsetting.Name),
                                Activator.CreateInstance(t, setting.Id));
                        }
                    }

                    var attribute = t.GetTypeInfo().GetCustomAttribute<IdentifyCacheAttribute>();
                    if (attribute != null)
                        RpcServiceResolver.Current.Register(attribute.Name.ToString(),
                            Activator.CreateInstance(t));
                }
            }
            catch
            {
                // ignored
            }
        }

        public object GetTypedPropertyValue(Property obj)
        {
            var mapCollections = obj.Maps;
            if (mapCollections != null && mapCollections.Any())
            {
                var results = new List<object>();
                foreach (var map in mapCollections)
                {
                    object items = null;
                    if (map.Properties != null) items = map.Properties.Select(p => GetTypedPropertyValue(p)).ToArray();
                    results.Add(new
                    {
                        Name = Convert.ChangeType(obj.Name, typeof(string)),
                        Value = Convert.ChangeType(map.Name, typeof(string)),
                        Items = items
                    });
                }

                return results;
            }

            if (!string.IsNullOrEmpty(obj.Value))
            {
                return new
                {
                    Name = Convert.ChangeType(obj.Name ?? "", typeof(string)),
                    Value = Convert.ChangeType(obj.Value, typeof(string)),
                };
            }

            if (!string.IsNullOrEmpty(obj.Ref))
                return Convert.ChangeType(obj.Ref, typeof(string));

            return null;
        }

        private void InitSettingMethod()
        {
            var settings = _cacheWrapperSetting.CachingSettings.Where(p => !string.IsNullOrEmpty(p.InitMethod));
            foreach (var setting in settings)
            {
                var bindingInstance = RpcServiceResolver.Current.GetService(Type.GetType(setting.Class, true), setting.Id);
                bindingInstance.GetType().GetMethod(setting.InitMethod, BindingFlags.InvokeMethod)
                    ?.Invoke(bindingInstance, new object[] { });
            }
        }
    }
}