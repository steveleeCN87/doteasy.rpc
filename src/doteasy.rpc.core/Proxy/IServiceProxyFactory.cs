using System;

namespace DotEasy.Rpc.Core.Proxy
{
    /// <summary>
    /// 一个抽象的服务代理工厂
    /// </summary>
    public interface IServiceProxyFactory
    {
        /// <summary>
        /// 创建服务代理
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <returns>服务代理实例</returns>
        object CreateProxy(Type proxyType);
    }
}