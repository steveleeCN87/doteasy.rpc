using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Consul;
using DotEasy.Rpc.Consul;
using DotEasy.Rpc.Core.Communally.Entitys.Address;
using DotEasy.Rpc.Core.Server;
using DotEasy.Rpc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Entry
{
    public class BaseServer
    {
        
        private readonly ContainerBuilder _serviceCollection = new ContainerBuilder();
        private readonly ConsulRpcOptionsConfiguration _consulRpcOptionsConfiguration = new ConsulRpcOptionsConfiguration();

        public delegate void RegisterEventHandler(ContainerBuilder serviceCollection);

        public event RegisterEventHandler RegisterEvent;

        /// <summary>
        /// 采用ConsulRpcOptionsConfiguration的构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public BaseServer(ConsulRpcOptionsConfiguration configuration)
        {
            if (configuration != null) _consulRpcOptionsConfiguration = configuration;
            Builder();
        }

        /// <summary>
        /// 采用IConfiguration的构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public BaseServer(IConfiguration configuration)
        {
            var configuration1 = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _consulRpcOptionsConfiguration = new ConsulRpcOptionsConfiguration()
            {
                HostingAndRpcHealthCheck = configuration1["Hosting.And.Rpc.Health.Check"],
                GRpc = new Consul.Rpc
                {
                    Ip = configuration1["Rpc:IP"],
                    Port = int.Parse(configuration1["Rpc:Port"])
                },
                ServiceDescriptors = new ServiceDescriptor
                {
                    Name = configuration1["ServiceDescriptor:Name"]
                },
                ConsulRegister = new ConsulRegister
                {
                    Ip = configuration1["ConsulRegister:IP"],
                    Port = int.Parse(configuration1["ConsulRegister:Port"]),
                    Timeout = int.Parse(configuration1["ConsulRegister:Timeout"])
                },
                ConsulClientConfiguration = new ConsulClientConfiguration
                {
                    Address = new Uri(
                        $"http://{configuration1["ConsulRegister:IP"]}" +
                        ":" +
                        $"{int.Parse(configuration1["ConsulRegister:Port"])}")
                }
            };
            Builder();
        }

        private void Builder()
        {
            _serviceCollection
                .AddLogging()
                .AddRpcCore()
                .AddService()
                .UseDotNettyTransport()
                .UseConsulRouteManager(_consulRpcOptionsConfiguration);
        }

        /// <summary>
        /// 启动rpc服务端
        /// </summary>
        public void Start()
        {
            RegisterEvent?.Invoke(_serviceCollection);
            
            var serviceProvider = _serviceCollection.Build();
            serviceProvider.Resolve<ILoggerFactory>().AddConsole((c, l) => (int) l >= 0);
            var serviceEntryManager = serviceProvider.Resolve<IServiceEntryManager>();
            var addressDescriptors = serviceEntryManager.GetEntries().Select(i => new ServiceRoute
            {
                Address = new[]
                {
                    new IpAddressModel
                    {
                        Ip = _consulRpcOptionsConfiguration.GRpc.Ip,
                        Port = _consulRpcOptionsConfiguration.GRpc.Port
                    }
                },
                ServiceDescriptor = i.Descriptor
            });

            var serviceRouteManager = serviceProvider.Resolve<IServiceRouteManager>();
            serviceRouteManager.SetRoutesAsync(addressDescriptors).Wait();

            var serviceHost = serviceProvider.Resolve<IServiceHost>();

            Task.Factory.StartNew(async () =>
            {
                await serviceHost.StartAsync(
                    new IPEndPoint(
                        IPAddress.Parse(_consulRpcOptionsConfiguration.GRpc.Ip),
                        _consulRpcOptionsConfiguration.GRpc.Port)
                );
            });
        }
    }
}