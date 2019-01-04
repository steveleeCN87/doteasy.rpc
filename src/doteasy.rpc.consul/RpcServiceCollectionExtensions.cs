using DotEasy.Rpc.Core;
using DotEasy.Rpc.Core.DependencyResolver;
using DotEasy.Rpc.Core.DependencyResolver.Builder;
using DotEasy.Rpc.Core.Routing;
using DotEasy.Rpc.Core.Runtime.Communally.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Consul
{
    public static class RpcServiceCollectionExtensions
    {
        public static IRpcBuilder UseConsulRouteManager(this IRpcBuilder builder,
            RpcOptionsConfiguration rpcOptionsConfiguration)
        {
            return builder.UseRouteManager(provider =>
                new ConsulServiceRouteManager(
                    rpcOptionsConfiguration,
                    provider.GetRequiredService<ISerializer<byte[]>>(),
                    provider.GetRequiredService<ISerializer<string>>(),
                    provider.GetRequiredService<IServiceRouteFactory>(),
                    provider.GetRequiredService<ILogger<ConsulServiceRouteManager>>()
                ));
        }
    }
}