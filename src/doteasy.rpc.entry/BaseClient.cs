using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Consul;
using DotEasy.Rpc.Consul;
using DotEasy.Rpc.Proxy;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Entry
{
    public class BaseClient
    {
        private ContainerBuilder _serviceCollection = new ContainerBuilder();

        private readonly IContainer _serviceProvider;

        public delegate void RegisterEventHandler(ContainerBuilder serviceCollection);

        public event RegisterEventHandler RegisterEvent;

        /// <summary>
        /// 默认内用内部配置的构造函数
        /// </summary>
        protected BaseClient()
        {
            _serviceCollection
                .AddLogging()
                .AddClient()
                .UseDotNettyTransport()
                .UseConsulRouteManager(new ConsulRpcOptionsConfiguration
                {
                    ConsulClientConfiguration = new ConsulClientConfiguration
                        {Address = new Uri("http://127.0.0.1:8500")}
                });

            _serviceProvider = _serviceCollection.Build();
        }

        /// <summary>
        /// 默认内用urlAddress的构造函数
        /// </summary>
        /// <param name="urlAddress"></param>
        public BaseClient(string urlAddress)
        {
            _serviceCollection
                .AddLogging()
                .AddClient()
                .UseDotNettyTransport()
                .UseConsulRouteManager(new ConsulRpcOptionsConfiguration
                {
                    ConsulClientConfiguration = new ConsulClientConfiguration {Address = new Uri(urlAddress)}
                });

            _serviceProvider = _serviceCollection.Build();
        }

        /// <summary>
        /// 代理生成预编译的客户端
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T Proxy<T>()
        {
            RegisterEvent?.Invoke(_serviceCollection);

            _serviceProvider.Resolve<ILoggerFactory>().AddConsole((c, l) => (int) l >= 4);

            var serviceProxyGenerate = _serviceProvider.Resolve<IServiceProxyGenerater>();
            var serviceProxyFactory = _serviceProvider.Resolve<IServiceProxyFactory>();

            return serviceProxyFactory.CreateProxy<T>(
                serviceProxyGenerate.GenerateProxys(new[] {typeof(T)})
                    .ToArray()
                    .Single(typeof(T).GetTypeInfo().IsAssignableFrom));
        }
    }
}