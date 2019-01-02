using System;
using System.Linq;
using System.Reflection;
using DotEasy.Rpc.Runtime.Client;
using DotEasy.Rpc.Runtime.Communally.Convertibles;

namespace DotEasy.Rpc.Proxy.Impl
{
    /// <summary>
    /// 默认的服务代理工厂实现
    /// </summary>
    public class ServiceProxyFactory : IServiceProxyFactory
    {
        private readonly IRemoteInvokeService _remoteInvokeService;
        private readonly ITypeConvertibleService _typeConvertibleService;

        public ServiceProxyFactory(IRemoteInvokeService remoteInvokeService, ITypeConvertibleService typeConvertibleService)
        {
            _remoteInvokeService = remoteInvokeService;
            _typeConvertibleService = typeConvertibleService;
        }

        /// <summary>
        /// 创建服务代理
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <returns>服务代理实例</returns>
        public object CreateProxy(Type proxyType)
        {
            return proxyType.GetTypeInfo().GetConstructors().First().Invoke(
                new object[] {_remoteInvokeService, _typeConvertibleService}
            );
        }
    }
}