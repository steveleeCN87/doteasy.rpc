using System;
using System.Linq;
using System.Reflection;
using Consul;
using DotEasy.Rpc.Core.DependencyResolver;
using DotEasy.Rpc.Core.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Consul.Entry
{
    public class ClientBase
    {
        private readonly ServiceCollection _serviceCollection = new ServiceCollection();
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 默认内用内部配置的构造函数
        /// </summary>
        protected ClientBase()
        {
            _serviceCollection
                .AddLogging()
                .AddClient()
                .UseDotNettyTransport()
                .UseConsulRouteManager(new ConsulRpcOptionsConfiguration
                {
                    ConsulClientConfiguration = new ConsulClientConfiguration {Address = new Uri("http://127.0.0.1:8500")}
                });

            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// 代理生成预编译的客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T Proxy<T>()
        {
            _serviceProvider.GetRequiredService<ILoggerFactory>().AddConsole((c, l) => (int) l >= 4);

            var serviceProxyGenerate = _serviceProvider.GetRequiredService<IServiceProxyGenerater>();
            var serviceProxyFactory = _serviceProvider.GetRequiredService<IServiceProxyFactory>();

            return serviceProxyFactory.CreateProxy<T>(
                serviceProxyGenerate.GenerateProxys(new[] {typeof(T)})
                    .ToArray()
                    .Single(typeof(T).GetTypeInfo().IsAssignableFrom));
        }
    }
}