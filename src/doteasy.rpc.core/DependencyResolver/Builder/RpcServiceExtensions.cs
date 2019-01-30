using System;
using System.Linq;
using DotEasy.Rpc.Core.ApiGateway.OAuth;
using DotEasy.Rpc.Core.ApiGateway.OAuth.Impl;
using DotEasy.Rpc.Core.Attributes;
using DotEasy.Rpc.Core.Cache;
using DotEasy.Rpc.Core.Cache.Impl.Redis;
using DotEasy.Rpc.Core.Proxy;
using DotEasy.Rpc.Core.Proxy.Impl;
using DotEasy.Rpc.Core.Routing;
using DotEasy.Rpc.Core.Routing.Impl;
using DotEasy.Rpc.Core.Runtime.Client;
using DotEasy.Rpc.Core.Runtime.Client.Address.Resolvers;
using DotEasy.Rpc.Core.Runtime.Client.Address.Resolvers.Implementation;
using DotEasy.Rpc.Core.Runtime.Client.Address.Resolvers.Implementation.Selectors;
using DotEasy.Rpc.Core.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using DotEasy.Rpc.Core.Runtime.Client.HealthChecks;
using DotEasy.Rpc.Core.Runtime.Client.HealthChecks.Implementation;
using DotEasy.Rpc.Core.Runtime.Client.Implementation;
using DotEasy.Rpc.Core.Runtime.Communally.Convertibles;
using DotEasy.Rpc.Core.Runtime.Communally.Convertibles.Impl;
using DotEasy.Rpc.Core.Runtime.Communally.IdGenerator;
using DotEasy.Rpc.Core.Runtime.Communally.IdGenerator.Impl;
using DotEasy.Rpc.Core.Runtime.Communally.Serialization;
using DotEasy.Rpc.Core.Runtime.Communally.Serialization.Implementation;
using DotEasy.Rpc.Core.Runtime.Server;
using DotEasy.Rpc.Core.Runtime.Server.Impl;
using DotEasy.Rpc.Core.Transport;
using DotEasy.Rpc.Core.Transport.Codec;
using DotEasy.Rpc.Core.Transport.Codec.Implementation;
using DotEasy.Rpc.Core.Transport.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Core.DependencyResolver.Builder
{
    public static class RpcServiceExtensions
    {
        /// <summary>
        /// 添加Json序列化支持
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        private static IRpcBuilder AddJsonSerialization(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<ISerializer<string>, JsonSerializer>();
            services.AddSingleton<ISerializer<byte[]>, StringByteArraySerializer>();
            services.AddSingleton<ISerializer<object>, StringObjectSerializer>();

            return builder;
        }

        #region RouteManager

        /// <summary>
        /// 设置服务路由管理者
        /// </summary>
        /// <typeparam name="T">服务路由管理者实现</typeparam>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRouteManager<T>(this IRpcBuilder builder) where T : class, IServiceRouteManager
        {
            builder.Services.AddSingleton<IServiceRouteManager, T>();
            return builder;
        }

        /// <summary>
        /// 设置服务路由管理者
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="factory">服务路由管理者实例工厂</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRouteManager(this IRpcBuilder builder, Func<IServiceProvider, IServiceRouteManager> factory)
        {
            builder.Services.AddSingleton(factory);
            return builder;
        }

        /// <summary>
        /// 设置服务路由管理者
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="instance">服务路由管理者实例</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRouteManager(this IRpcBuilder builder, IServiceRouteManager instance)
        {
            builder.Services.AddSingleton(instance);
            return builder;
        }

        #endregion RouteManager

        /// <summary>
        /// 设置共享文件路由管理者
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseSharedFileRouteManager(this IRpcBuilder builder, string filePath)
        {
            return builder.UseRouteManager(provider =>
                new SharedFileServiceRouteManager(
                    filePath,
                    provider.GetRequiredService<ISerializer<string>>(),
                    provider.GetRequiredService<IServiceRouteFactory>(),
                    provider.GetRequiredService<ILogger<SharedFileServiceRouteManager>>()
                ));
        }

        #region AddressSelector

        /// <summary>
        /// 设置服务地址选择器
        /// </summary>
        /// <typeparam name="T">地址选择器实现类型</typeparam>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseAddressSelector<T>(this IRpcBuilder builder) where T : class, IAddressSelector
        {
            builder.Services.AddSingleton<IAddressSelector, T>();
            return builder;
        }

        /// <summary>
        /// 设置服务地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="factory">服务地址选择器实例工厂</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseAddressSelector(this IRpcBuilder builder,
            Func<IServiceProvider, IAddressSelector> factory)
        {
            builder.Services.AddSingleton(factory);

            return builder;
        }

        /// <summary>
        /// 设置服务地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="instance">地址选择器实例</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseAddressSelector(this IRpcBuilder builder, IAddressSelector instance)
        {
            builder.Services.AddSingleton(instance);

            return builder;
        }

        #endregion AddressSelector

        /// <summary>
        /// 使用轮询的地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UsePollingAddressSelector(this IRpcBuilder builder)
        {
            builder.Services.AddSingleton<IAddressSelector, PollingAddressSelector>();
            return builder;
        }

        /// <summary>
        /// 使用随机的地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRandomAddressSelector(this IRpcBuilder builder)
        {
            builder.Services.AddSingleton<IAddressSelector, RandomAddressSelector>();
            return builder;
        }

        #region Codec Factory

        /// <summary>
        /// 使用编解码器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="codecFactory"></param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseCodec(this IRpcBuilder builder, ITransportMessageCodecFactory codecFactory)
        {
            builder.Services.AddSingleton(codecFactory);
            return builder;
        }

        /// <summary>
        /// 使用编解码器
        /// </summary>
        /// <typeparam name="T">编解码器工厂实现类型</typeparam>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseCodec<T>(this IRpcBuilder builder) where T : class, ITransportMessageCodecFactory
        {
            builder.Services.AddSingleton<ITransportMessageCodecFactory, T>();
            return builder;
        }

        /// <summary>
        /// 使用编解码器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="codecFactory">编解码器工厂创建委托</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseCodec(this IRpcBuilder builder,
            Func<IServiceProvider, ITransportMessageCodecFactory> codecFactory)
        {
            builder.Services.AddSingleton(codecFactory);

            return builder;
        }

        #endregion Codec Factory

        /// <summary>
        /// 使用Json编解码器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseJsonCodec(this IRpcBuilder builder)
        {
            return builder.UseCodec<JsonTransportMessageCodecFactory>();
        }

        /// <summary>
        /// 添加客户端运行时服务
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder AddClientRuntime(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<IHealthCheckService, DefaultHealthCheckService>();
            services.AddSingleton<IAddressResolver, DefaultAddressResolver>();
            services.AddSingleton<IRemoteInvokeService, RemoteInvokeService>();

            return builder.UsePollingAddressSelector();
        }

        /// <summary>
        /// 添加服务运行时服务
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder AddService(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<IServiceEntryFactory, ServiceEntryFactory>();
            services.AddSingleton<IServiceEntryProvider>(provider =>
                new AttributeServiceEntryProvider(
                    AppDomain.CurrentDomain.GetAssemblies().AsParallel().Where(i => i.IsDynamic == false).SelectMany(i => i.ExportedTypes)
                        .ToArray(),
                    provider.GetRequiredService<IServiceEntryFactory>(),
                    provider.GetRequiredService<ILogger<AttributeServiceEntryProvider>>()));
            services.AddSingleton<IServiceEntryManager, DefaultServiceEntryManager>();
            services.AddSingleton<IServiceEntryLocate, DefaultServiceEntryLocate>();
            services.AddSingleton<IServiceExecutor, DefaultServiceExecutor>();

            return builder;
        }

        /// <summary>
        /// 添加RPC核心服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder AddRpcCore(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ICacheProvider, DefaultRedisCacheProvider>();
            services.AddSingleton<IServiceIdGenerator, DefaultServiceIdGenerator>();
            services.AddSingleton<ITypeConvertibleProvider, DefaultTypeConvertibleProvider>();
            services.AddSingleton<ITypeConvertibleService, DefaultTypeConvertibleService>();
            services.AddSingleton<IServiceRouteFactory, DefaultServiceRouteFactory>();

            return new RpcBuilder(services)
                .AddJsonSerialization()
                .UseJsonCodec();
        }

        public static IRpcBuilder AddClientProxy(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<IServiceProxyGenerater, ServiceProxyGenerater>();
            services.AddSingleton<IServiceProxyFactory, ServiceProxyFactory>();

            return builder;
        }

        public static IRpcBuilder AddClient(this IServiceCollection services)
        {
            return services
                .AddRpcCore()
                .AddClientRuntime()
                .AddClientProxy();
        }

        /// <summary>
        /// 使用DotNetty进行传输。
        /// </summary>
        /// <param name="builder">Rpc服务构建者。</param>
        /// <returns>Rpc服务构建者。</returns>
        public static IRpcBuilder UseDotNettyTransport(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.AddSingleton<ITransportClientFactory, DefaultDotNettyTransportClientFactory>();

            services.AddSingleton<DefaultDotNettyServerMessageListener>();

            services.AddSingleton<IServiceHost, DefaultServiceHost>(
                provider => new DefaultServiceHost(
                    async endPoint =>
                    {
                        var messageListener = provider.GetRequiredService<DefaultDotNettyServerMessageListener>();
                        await messageListener.StartAsync(endPoint);
                        return messageListener;
                    },
                    provider.GetRequiredService<IServiceExecutor>()
                )
            );

            return builder;
        }

        /// <summary>
        /// 添加RPC服务认证 | 认证在服务端
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authorizationServerProvider"></param>
        /// <returns></returns>
        public static IRpcBuilder AddAuthentication(this IRpcBuilder builder, Type authorizationServerProvider = null)
        {
            var services = builder.Services;

            services.AddSingleton(typeof(IAuthorizationServerProvider), 
                authorizationServerProvider == null ? typeof(DefaultAuthorizationServerProvider) : authorizationServerProvider);

            return builder;
        }
    }
}