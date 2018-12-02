using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using DotEasy.Rpc.Core.Communally.Entitys.Messages;
using DotEasy.Rpc.Core.Server;
using DotEasy.Rpc.Transport.Codec;
using DotEasy.Rpc.Transport.InternalAdaper;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Transport.Impl
{
    /// <summary>
    /// 基于DotNetty的传输客户端工厂
    /// </summary>
    public class DefaultDotNettyTransportClientFactory : ITransportClientFactory, IDisposable
    {
        private readonly ITransportMessageEncoder _transportMessageEncoder;
        private readonly IServiceExecutor _serviceExecutor;
        private readonly ILogger<DefaultDotNettyTransportClientFactory> _logger;

        private readonly ConcurrentDictionary<EndPoint, Lazy<ITransportClient>> _clients =
            new ConcurrentDictionary<EndPoint, Lazy<ITransportClient>>();

        private readonly Bootstrap _bootstrap;

        private static readonly AttributeKey<IMessageSender> MessageSenderKey =
            AttributeKey<IMessageSender>.ValueOf(typeof(DefaultDotNettyTransportClientFactory), nameof(IMessageSender));

        private static readonly AttributeKey<IMessageListener> MessageListenerKey =
            AttributeKey<IMessageListener>.ValueOf(typeof(DefaultDotNettyTransportClientFactory),
                nameof(IMessageListener));

        private static readonly AttributeKey<EndPoint> OrigEndPointKey =
            AttributeKey<EndPoint>.ValueOf(typeof(DefaultDotNettyTransportClientFactory), nameof(EndPoint));


        public DefaultDotNettyTransportClientFactory(ITransportMessageCodecFactory codecFactory, ILogger<DefaultDotNettyTransportClientFactory> logger, 
            IServiceExecutor serviceExecutor = null)
        {
            _logger = logger;
            _transportMessageEncoder = codecFactory.GetEncoder();
            var transportMessageDecoder = codecFactory.GetDecoder();
            _serviceExecutor = serviceExecutor;
            _bootstrap = GetBootstrap();
            _bootstrap.Handler(new ActionChannelInitializer<ISocketChannel>(c =>
            {
                var pipeline = c.Pipeline;
                pipeline.AddLast(new LengthFieldPrepender(4));
                pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                pipeline.AddLast(new TransportMessageChannelHandlerDecodeAdapter(transportMessageDecoder));
                pipeline.AddLast(new DefaultChannelHandler(this));
            }));
        }


        /// <summary>
        /// 创建客户端
        /// </summary>
        /// <param name="endPoint">终结点</param>
        /// <returns>传输客户端实例</returns>
        public ITransportClient CreateClient(EndPoint endPoint)
        {
            var key = endPoint;
            
                Console.WriteLine($"准备为服务端地址：{key}创建客户端。");
            
            try
            {
                return _clients.GetOrAdd(key
                    , k => new Lazy<ITransportClient>(() =>
                        {
                            var bootstrap = _bootstrap;
                            var channel = bootstrap.ConnectAsync(k).Result;

                            var messageListener = new MessageListener();
                            channel.GetAttribute(MessageListenerKey).Set(messageListener);
                            var messageSender =
                                new DefaultDotNettyMessageClientSender(_transportMessageEncoder, channel);
                            channel.GetAttribute(MessageSenderKey).Set(messageSender);
                            channel.GetAttribute(OrigEndPointKey).Set(k);

                            var client = new TransportClient(messageSender, messageListener, _serviceExecutor, _logger);
                            return client;
                        }
                    )).Value;
            }
            catch
            {
                _clients.TryRemove(key, out _);
                throw;
            }
        }


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            foreach (var client in _clients.Values.Where(i => i.IsValueCreated))
            {
                (client.Value as IDisposable)?.Dispose();
            }
        }

        private static Bootstrap GetBootstrap()
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Group(new MultithreadEventLoopGroup());

            return bootstrap;
        }

        public class DefaultChannelHandler : ChannelHandlerAdapter
        {
            private readonly DefaultDotNettyTransportClientFactory _factory;

            public DefaultChannelHandler(DefaultDotNettyTransportClientFactory factory)
            {
                _factory = factory;
            }

            public override void ChannelInactive(IChannelHandlerContext context)
            {
                _factory._clients.TryRemove(context.Channel.GetAttribute(OrigEndPointKey).Get(), out _);
            }

            public override void ChannelRead(IChannelHandlerContext context, object message)
            {
                var transportMessage = message as TransportMessage;

                var messageListener = context.Channel.GetAttribute(MessageListenerKey).Get();
                var messageSender = context.Channel.GetAttribute(MessageSenderKey).Get();
                messageListener.OnReceived(messageSender, transportMessage);
            }
        }
    }
}