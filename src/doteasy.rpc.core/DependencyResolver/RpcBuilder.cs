using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace DotEasy.Rpc.Core.DependencyResolver
{
    /// <summary>
    /// 默认的Rpc服务构建者
    /// </summary>
    internal sealed class RpcBuilder : IRpcBuilder
    {
        public IServiceCollection Services { get; }
        public ContainerBuilder AutofacContainer { get; }

        public RpcBuilder(ContainerBuilder services)
        {
            AutofacContainer = services ?? throw new ArgumentNullException(nameof(services));
        }

        public RpcBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
    }
}