using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Consul;
using DotEasy.Rpc.Core.DependencyResolver.Builder;
using DotEasy.Rpc.Core.Proxy;
using DotEasy.Rpc.Core.Runtime.Client;
using DotEasy.Rpc.Core.Runtime.Communally.Convertibles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Consul.Entry
{
    public static class ClientProxy
    {
        private static ServiceCollection serviceCollection = new ServiceCollection();
        private const int Loglevel = 4;

        public static IRemoteInvokeService IRemoteInvokeService;
        public static ITypeConvertibleService ITypeConvertibleService;
        
        /// <summary>
        /// 代理生成预编译的客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Generate<T>(Uri consulUrl)
        {
            serviceCollection.AddLogging().AddClient().UseDotNettyTransport().UseConsulRouteManager(new RpcOptionsConfiguration
            {
                ConsulClientConfiguration = new ConsulClientConfiguration {Address = consulUrl}
            });

            return Proxy<T>();
        }

        /// <summary>
        /// 代理生成预编译的客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Generate<T>(Uri consulUrl, string accessToken)
        {
            serviceCollection
                .AddLogging()
                .AddClient()
                .UseDotNettyTransport()
                .UseConsulRouteManager(new RpcOptionsConfiguration
                {
                    ConsulClientConfiguration = new ConsulClientConfiguration {Address = consulUrl}
                });

            return Proxy<T>(accessToken);
        }

        /// <summary>
        /// 默认使用应用程序下包含interfaces的库作为扫描基础
        /// </summary>
        /// <param name="consulUrl"></param>
        /// <returns></returns>
        public static List<dynamic> GenerateAll(Uri consulUrl)
        {
            serviceCollection
                .AddLogging()
                .AddClient()
                .UseDotNettyTransport()
                .UseConsulRouteManager(new RpcOptionsConfiguration
                {
                    ConsulClientConfiguration = new ConsulClientConfiguration {Address = consulUrl}
                });

            return ProxyAll();
        }

        private static List<dynamic> ProxyAll()
        {
            var serviceProvider = Builder();
            var serviceProxyGenerate = serviceProvider.GetRequiredService<IServiceProxyGenerater>();
            var serviceProxyFactory = serviceProvider.GetRequiredService<IServiceProxyFactory>();

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var files = new DirectoryInfo(path).GetFiles();
            var types = (from file in files
                where file.Name.Contains("interfaces") && file.Extension.Contains("dll")
                from type in Assembly.LoadFrom(file.ToString()).GetExportedTypes()
                where type.IsInterface
                select type).ToList();

            var objs = serviceProxyGenerate.GenerateProxys(types)
                .Select(proxy => serviceProxyFactory.CreateProxy(proxy))
                .Cast<dynamic>().ToList();
            
            IRemoteInvokeService = serviceProvider.GetRequiredService<IRemoteInvokeService>();
            ITypeConvertibleService = serviceProvider.GetRequiredService<ITypeConvertibleService>();
            
            return objs;
        }

        #region 私有方法

        private static ServiceProvider Builder()
        {
            return serviceCollection.BuildServiceProvider();
        }

        private static T Proxy<T>(string accessToken = "")
        {
            var serviceProvider = Builder();
#if DEBUG
            serviceProvider.GetRequiredService<ILoggerFactory>().AddConsole((c, l) => (int) l >= Loglevel);
#endif
            var serviceProxyGenerate = serviceProvider.GetRequiredService<IServiceProxyGenerater>();
            var serviceProxyFactory = serviceProvider.GetRequiredService<IServiceProxyFactory>();

            if (accessToken == "")
            {
                return serviceProxyFactory
                    .CreateProxy<T>(serviceProxyGenerate.GenerateProxys(new[] {typeof(T)})
                        .ToArray()
                        .Single(typeof(T).GetTypeInfo().IsAssignableFrom));
            }

            return serviceProxyFactory
                .CreateProxy<T>(serviceProxyGenerate.GenerateProxys(new[] {typeof(T)}, accessToken)
                    .ToArray()
                    .Single(typeof(T).GetTypeInfo().IsAssignableFrom));
        }

        #endregion
    }
}