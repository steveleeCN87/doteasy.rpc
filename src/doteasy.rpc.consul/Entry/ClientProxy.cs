using System;
using System.Linq;
using System.Reflection;
using Consul;
using DotEasy.Rpc.Core.DependencyResolver.Builder;
using DotEasy.Rpc.Core.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Consul.Entry
{
    public static class ClientProxy
    {
        private static ServiceCollection serviceCollection = new ServiceCollection();
        private const int Loglevel = 4;

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