using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotEasy.Rpc.Core.DependencyResolver.Builder
{
    /// <summary>
    /// 默认的Rpc服务构建者
    /// </summary>
    internal sealed class RpcBuilder : IRpcBuilder
    {
        public IServiceCollection Services { get; }

        public RpcBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        
    }
}