using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotEasy.Rpc
{
    /// <summary>
    /// 默认的Rpc服务构建者
    /// </summary>
    internal sealed class RpcBuilder : IRpcBuilder
    {
        public RpcBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// 服务集合
        /// </summary>
        public IServiceCollection Services { get; }
    }
}