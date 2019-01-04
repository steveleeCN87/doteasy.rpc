using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Consul;
using DotEasy.Rpc.Core.DependencyResolver;
using DotEasy.Rpc.Core.DependencyResolver.Builder;
using DotEasy.Rpc.Core.Routing;
using DotEasy.Rpc.Core.Runtime.Communally.Entitys.Address;
using DotEasy.Rpc.Core.Runtime.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Consul.Entry
{
    public class ServerBase
    {
        private readonly ServiceCollection _serviceCollection = new ServiceCollection();
        private readonly RpcOptionsConfiguration _rpcOptionsConfiguration;

        public delegate void RegisterEventHandler(ServiceCollection serviceCollection);

        public event RegisterEventHandler RegisterEvent;

        /// <summary>
        /// 采用IConfiguration的构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public ServerBase(IConfiguration configuration)
        {
            var configuration1 = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _rpcOptionsConfiguration = new RpcOptionsConfiguration
            {
                HostingAndRpcHealthCheck = configuration1["Hosting.And.Rpc.Health.Check"],
                GRpc = new Rpc
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
                .UseConsulRouteManager(_rpcOptionsConfiguration);
        }

        /// <summary>
        /// 启动rpc服务端
        /// </summary>
        public void Start()
        {
            RegisterEvent?.Invoke(_serviceCollection);

            var autofacBuilder = new ContainerBuilder();
            autofacBuilder.Populate(_serviceCollection);
            var container = autofacBuilder.Build();

            container.Resolve<ILoggerFactory>().AddConsole((c, l) => (int) l >= 0);
            var serviceEntryManager = container.Resolve<IServiceEntryManager>();
            var addressDescriptors = serviceEntryManager.GetEntries().Select(i => new ServiceRoute
            {
                Address = new[]
                {
                    new IpAddressModel
                    {
                        Ip = _rpcOptionsConfiguration.GRpc.Ip,
                        Port = _rpcOptionsConfiguration.GRpc.Port
                    }
                },
                ServiceDescriptor = i.Descriptor
            });
            var serviceRouteManager = container.Resolve<IServiceRouteManager>();
            serviceRouteManager.SetRoutesAsync(addressDescriptors).Wait();
            var serviceHost = container.Resolve<IServiceHost>();

            Task.Factory.StartNew(async () =>
            {
                await serviceHost.StartAsync(
                    new IPEndPoint(
                        IPAddress.Parse(_rpcOptionsConfiguration.GRpc.Ip),
                        _rpcOptionsConfiguration.GRpc.Port)
                );
            });
        }
    }
}