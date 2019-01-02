using System;
using System.Linq;
using Autofac;
using DotEasy.Rpc.Attributes;
using DotEasy.Rpc.Proxy;
using DotEasy.Rpc.Proxy.Impl;
using DotEasy.Rpc.Routing;
using DotEasy.Rpc.Routing.Impl;
using DotEasy.Rpc.Runtime.Client;
using DotEasy.Rpc.Runtime.Client.Address.Resolvers;
using DotEasy.Rpc.Runtime.Client.Address.Resolvers.Implementation;
using DotEasy.Rpc.Runtime.Client.Address.Resolvers.Implementation.Selectors;
using DotEasy.Rpc.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using DotEasy.Rpc.Runtime.Client.HealthChecks;
using DotEasy.Rpc.Runtime.Client.HealthChecks.Implementation;
using DotEasy.Rpc.Runtime.Client.Implementation;
using DotEasy.Rpc.Runtime.Communally.Convertibles;
using DotEasy.Rpc.Runtime.Communally.Convertibles.Impl;
using DotEasy.Rpc.Runtime.Communally.IdGenerator;
using DotEasy.Rpc.Runtime.Communally.IdGenerator.Impl;
using DotEasy.Rpc.Runtime.Communally.Serialization;
using DotEasy.Rpc.Runtime.Communally.Serialization.Implementation;
using DotEasy.Rpc.Runtime.Server;
using DotEasy.Rpc.Runtime.Server.Impl;
using DotEasy.Rpc.Transport;
using DotEasy.Rpc.Transport.Codec;
using DotEasy.Rpc.Transport.Codec.Implementation;
using DotEasy.Rpc.Transport.Impl;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc
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
            services.RegisterType<JsonSerializer>().As<ISerializer<string>>();
            services.RegisterType<StringByteArraySerializer>().As<ISerializer<byte[]>>();
            services.RegisterType<StringObjectSerializer>().As<ISerializer<object>>();
            return builder;
        }

        /// <summary>
        /// 设置服务路由管理者
        /// </summary>
        /// <typeparam name="T">服务路由管理者实现</typeparam>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRouteManager<T>(this IRpcBuilder builder) where T : class, IServiceRouteManager
        {
            builder.Services.RegisterType<T>().As<IServiceRouteManager>();
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
            builder.Services.RegisterInstance(factory);
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
            builder.Services.RegisterInstance(instance);
            return builder;
        }

        /// <summary>
        /// 设置服务地址选择器
        /// </summary>
        /// <typeparam name="T">地址选择器实现类型</typeparam>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseAddressSelector<T>(this IRpcBuilder builder) where T : class, IAddressSelector
        {
            builder.Services.RegisterType<T>().As<IAddressSelector>();
            return builder;
        }

        /// <summary>
        /// 设置服务地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="factory">服务地址选择器实例工厂</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseAddressSelector(this IRpcBuilder builder, Func<IServiceProvider, IAddressSelector> factory)
        {
            builder.Services.RegisterInstance(factory);
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
            builder.Services.RegisterInstance(instance);
            return builder;
        }

        /// <summary>
        /// 使用轮询的地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UsePollingAddressSelector(this IRpcBuilder builder)
        {
            builder.Services.RegisterType<PollingAddressSelector>().As<IAddressSelector>();
            return builder;
        }

        /// <summary>
        /// 使用随机的地址选择器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseRandomAddressSelector(this IRpcBuilder builder)
        {
            builder.Services.RegisterType<RandomAddressSelector>().As<IAddressSelector>();
            return builder;
        }

        /// <summary>
        /// 使用编解码器
        /// </summary>
        /// <param name="builder">Rpc服务构建者</param>
        /// <param name="codecFactory"></param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder UseCodec(this IRpcBuilder builder, ITransportMessageCodecFactory codecFactory)
        {
            builder.Services.RegisterInstance(codecFactory);
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
            builder.Services.RegisterType<T>().As<ITransportMessageCodecFactory>();
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
            builder.Services.RegisterInstance(codecFactory);
            return builder;
        }

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
            services.RegisterType<DefaultHealthCheckService>().As<IHealthCheckService>();
            services.RegisterType<DefaultAddressResolver>().As<IAddressResolver>();
            services.RegisterType<RemoteInvokeService>().As<IRemoteInvokeService>();
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
            services.RegisterType<ServiceEntryFactory>().As<IServiceEntryFactory>();
            services.Register<IServiceEntryProvider>(provider =>
                new AttributeServiceEntryProvider(
                    AppDomain.CurrentDomain.GetAssemblies().AsParallel().Where(i => i.IsDynamic == false).SelectMany(i => i.ExportedTypes).ToArray(),
                    provider.Resolve<IServiceEntryFactory>(),
                    provider.Resolve<ILogger<AttributeServiceEntryProvider>>()));

            services.RegisterType<DefaultServiceEntryManager>().As<IServiceEntryManager>();
            services.RegisterType<DefaultServiceEntryLocate>().As<IServiceEntryLocate>();
            services.RegisterType<DefaultServiceExecutor>().As<IServiceExecutor>();

            return builder;
        }

        /// <summary>
        /// 添加RPC核心服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>Rpc服务构建者</returns>
        public static IRpcBuilder AddRpcCore(this ContainerBuilder services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.RegisterType<DefaultServiceIdGenerator>().As<IServiceIdGenerator>();
            services.RegisterType<DefaultTypeConvertibleProvider>().As<ITypeConvertibleProvider>();
            services.RegisterType<DefaultTypeConvertibleService>().As<ITypeConvertibleService>();
            services.RegisterType<DefaultServiceRouteFactory>().As<IServiceRouteFactory>();

            return new RpcBuilder(services).AddJsonSerialization().UseJsonCodec();
        }

        public static IRpcBuilder AddClientProxy(this IRpcBuilder builder)
        {
            var services = builder.Services;

            services.RegisterType<ServiceProxyFactory>().As<IServiceProxyFactory>();
            services.RegisterType<ServiceProxyGenerater>().As<IServiceProxyGenerater>();

            return builder;
        }

        public static IRpcBuilder AddClient(this ContainerBuilder services)
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

            services.RegisterType<DefaultDotNettyTransportClientFactory>().As<ITransportClientFactory>();
            services.RegisterType<DefaultDotNettyServerMessageListener>();
            services.RegisterType<DefaultServiceHost>().As<IServiceHost>();
            services.Register<IServiceHost>(r => new DefaultServiceHost(async endPoint =>
            {
                var messageListener = r.Resolve<DefaultDotNettyServerMessageListener>();
                await messageListener.StartAsync(endPoint);
                return messageListener;
            }, r.Resolve<IServiceExecutor>()));

            return builder;
        }

        /// <summary>
        /// 添加控制台输出
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ContainerBuilder AddLogging(this ContainerBuilder builder)
        {
            builder.RegisterType<LoggerFactory>().As<ILoggerFactory>();
//            builder.RegisterType<ILogger>();
            return builder;
        }
    }
}