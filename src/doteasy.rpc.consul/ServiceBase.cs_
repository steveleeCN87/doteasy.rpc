using System;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Consul
{
    public class ServiceBase
    {
        public ServiceBase()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddLogging()
                .AddClient()
                .UseDotNettyTransport()
                .UseConsulRouteManager(new ConsulRpcOptionsConfiguration
                {
                    ConsulClientConfiguration = new ConsulClientConfiguration
                    {
                        Address = new Uri("http://127.0.0.1:8500")
                    }
                });

            serviceCollection.BuildServiceProvider();

//            serviceProvider.GetRequiredService<ILoggerFactory>().AddConsole((console, logLevel) => (int) logLevel >= 0);
        }

        public ServiceBase(string ConsulUrlAddress)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddLogging()
                .AddClient()
                .UseDotNettyTransport()
                .UseConsulRouteManager(new ConsulRpcOptionsConfiguration
                {
                    ConsulClientConfiguration = new ConsulClientConfiguration
                    {
                        Address = new Uri(ConsulUrlAddress)
                    }
                });

            serviceCollection.BuildServiceProvider();
        }
    }
}