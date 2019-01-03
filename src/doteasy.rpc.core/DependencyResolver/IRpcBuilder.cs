using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace DotEasy.Rpc.Core.DependencyResolver
{
    /// <summary>
    /// Rpc服务构建者
    /// </summary>
    public interface IRpcBuilder
    {
        /// <summary>
        /// 默认服务集合
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Autofac容器
        /// </summary>
        ContainerBuilder AutofacContainer { get; }
    }
}