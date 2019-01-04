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
        public static T Generate<T>(string consulUrl)
        {
            serviceCollection.AddLogging().AddClient().UseDotNettyTransport().UseConsulRouteManager(new RpcOptionsConfiguration
            {
                ConsulClientConfiguration = new ConsulClientConfiguration {Address = new Uri(consulUrl)}
            });

            return Proxy<T>();
        }

        /// <summary>
        /// 代理生成预编译的客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Generate<T>(RpcOptionsConfiguration configuration)
        {
            serviceCollection.AddLogging().AddClient().UseDotNettyTransport().UseConsulRouteManager(configuration);
            return Proxy<T>();
        }

        /// <summary>
        /// 代理生成预编译的客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Generate<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 代理生成预编译的客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Generate<T>(object context)
        {
            throw new NotImplementedException();
        }


        #region 私有方法

        private static ServiceProvider Builder()
        {
            return serviceCollection.BuildServiceProvider();
        }

        private static T Proxy<T>()
        {
            var serviceProvider = Builder();
#if DEBUG
            serviceProvider.GetRequiredService<ILoggerFactory>().AddConsole((c, l) => (int) l >= Loglevel);
#endif
            var serviceProxyGenerate = serviceProvider.GetRequiredService<IServiceProxyGenerater>();
            var serviceProxyFactory = serviceProvider.GetRequiredService<IServiceProxyFactory>();
            return serviceProxyFactory.CreateProxy<T>(serviceProxyGenerate.GenerateProxys(new[] {typeof(T)})
                .ToArray()
                .Single(typeof(T).GetTypeInfo().IsAssignableFrom));
        }

        #endregion
    }
}