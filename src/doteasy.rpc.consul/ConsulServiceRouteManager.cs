using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using DotEasy.Rpc.Core.Communally.Serialization;
using DotEasy.Rpc.Routing;
using DotEasy.Rpc.Routing.Impl;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotEasy.Rpc.Consul
{
    public class ConsulServiceRouteManager : ServiceRouteManagerBase, IDisposable
    {
        private ConsulClient _consulClient;
        private readonly ConsulRpcOptionsConfiguration _rpcOptionsConfiguration;
        private readonly ConsulClientConfiguration _configInfo;
        private AgentServiceCheck _agentServiceCheck;
        private readonly ISerializer<byte[]> _byteSerializer;
        private readonly IServiceRouteFactory _serviceRouteFactory;
        private readonly ILogger<ConsulServiceRouteManager> _logger;
        private ServiceRoute[] _routes;
        private readonly ManualResetEvent _connectionWait = new ManualResetEvent(false);

        public ConsulServiceRouteManager(
            ConsulRpcOptionsConfiguration consulRpcOptionsConfiguration,
            ISerializer<byte[]> byteSerializer,
            ISerializer<string> stringSerializer,
            IServiceRouteFactory serviceRouteFactory,
            ILogger<ConsulServiceRouteManager> logger
        ) : base(stringSerializer)
        {
            _rpcOptionsConfiguration = consulRpcOptionsConfiguration;
            _configInfo = consulRpcOptionsConfiguration.ConsulClientConfiguration;
            _byteSerializer = byteSerializer;
            _serviceRouteFactory = serviceRouteFactory;
            _logger = logger;

            CreateConsul();
        }

        private void CreateConsul()
        {
            if (_consulClient != null) return;
            // 创建客户端
#pragma warning disable 618
            _consulClient = new ConsulClient(_configInfo);
#pragma warning restore 618
            if (_rpcOptionsConfiguration.ServiceDescriptors != null)
            {
                // 创建服务健康检查
                _agentServiceCheck = new AgentServiceCheck
                {
                    // 服务启动多久后注册
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 健康检查时间间隔，或者称为心跳间隔
                    Interval = TimeSpan.FromSeconds(5),
                    // 各服务的统一健康检查地址
                    HTTP = _rpcOptionsConfiguration.HostingAndRpcHealthCheck,
                    // 超时时间
                    Timeout = TimeSpan.FromSeconds(_rpcOptionsConfiguration.ConsulRegister.Timeout)
                };
            }
        }

        public override Task<IEnumerable<ServiceRoute>> GetRoutesAsync()
        {
            var task = new Task<IEnumerable<ServiceRoute>>(() =>
            {
//                if (_routes != null) _connectionWait.WaitOne();
                _logger.LogInformation("准备获取所有路由配置。");

                var allService = GetResponseData(
                    $"{_rpcOptionsConfiguration.ConsulClientConfiguration.Address}/v1/catalog/services"
                );
                // 去除默认的consul节点信息
                allService.Remove("consul");
                List<ServiceRouteDescriptor> serviceRouteDescriptors = new List<ServiceRouteDescriptor>();
                foreach (var keyValuePair in allService)
                {
                    var serviceMetaDictionary = GetResponseData(
                        $"{_rpcOptionsConfiguration.ConsulClientConfiguration.Address}/v1/catalog/service/"
                        + keyValuePair.Key);
                    var serviceMetaInfo = JsonConvert.DeserializeObject<ConsulServiceMetaInfo>(
                        JsonConvert.SerializeObject(serviceMetaDictionary)
                    );
                    serviceRouteDescriptors.Add(CreateServiceRouteDescriptor(serviceMetaInfo));
                }

                _routes = _serviceRouteFactory.CreateServiceRoutesAsync(serviceRouteDescriptors).Result.ToArray();
                return _routes;
            });
            task.Start();
            return task;
        }

        private ServiceRouteDescriptor CreateServiceRouteDescriptor(ConsulServiceMetaInfo consulServiceMetaInfo)
        {
            return new ServiceRouteDescriptor
            {
                AddressDescriptors = new List<ServiceAddressDescriptor>
                {
                    new ServiceAddressDescriptor
                    {
                        Type = "DotEasy.Rpc.Core.Communally.Entitys.Address.IpAddressModel",
                        Value = JsonConvert.SerializeObject(new Dictionary<string, dynamic>()
                        {
                            {"Ip", consulServiceMetaInfo.ServiceAddress},
                            {"Port", consulServiceMetaInfo.ServicePort}
                        })
                    }
                },
                ServiceDescriptor = new Core.Communally.Entitys.ServiceDescriptor
                {
                    Id = consulServiceMetaInfo.ServiceName,
                    Metadatas = JsonConvert.DeserializeObject<Dictionary<string, Object>>(
                        JsonConvert.SerializeObject(consulServiceMetaInfo)),
                }
            };
        }

        private Dictionary<string, object> GetResponseData(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var myResponseStream = response.Content.ReadAsStreamAsync().Result;
                    var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    var content = myStreamReader.ReadToEnd();
                    if (content.StartsWith("["))
                    {
                        return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(content)[0];
                    }

                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                }
            }

            return null;
        }

        public override Task ClearAsync()
        {
            _logger.LogInformation("准备清空所有路由配置");
            _routes.ToList().Clear();
            return Task.FromResult(0);
        }

        protected override Task SetRoutesAsync(IEnumerable<ServiceRouteDescriptor> routes)
        {
            _logger.LogInformation("准备添加服务路由");
            foreach (var serviceRoute in routes)
            {
                _consulClient.Agent.ServiceRegister(new AgentServiceRegistration
                {
                    Checks = new[] {_agentServiceCheck},
                    ID = Guid.NewGuid().ToString(),
                    Name = serviceRoute.ServiceDescriptor.Id,
                    Address = _rpcOptionsConfiguration.GRpc.Ip,
                    Port = _rpcOptionsConfiguration.GRpc.Port,
                    Tags = new[] {$"/{serviceRoute.ServiceDescriptor.Id}"}
                }).Wait();
            }

            _logger.LogInformation("服务路由添加成功");
            return GetRoutesAsync();
        }

        public void Dispose()
        {
            _connectionWait.Dispose();
            _consulClient.Dispose();
        }
    }
}