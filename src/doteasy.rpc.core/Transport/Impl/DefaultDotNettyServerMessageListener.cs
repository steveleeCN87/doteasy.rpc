using System;
using System.Net;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Runtime.Communally.Entitys.Messages;
using DotEasy.Rpc.Core.Transport.Codec;
using DotEasy.Rpc.Core.Transport.InternalAdaper;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Core.Transport.Impl
{
    /// <summary>
    /// 基于DotNetty的消息监听者
    /// </summary>
    public class DefaultDotNettyServerMessageListener : IMessageListener, IDisposable
    {
        private readonly ITransportMessageDecoder _transportMessageDecoder;
        private readonly ITransportMessageEncoder _transportMessageEncoder;
        private readonly ILogger _logger;
        private readonly int _lengthFieldPrepender = 4;
        private readonly int _basedFrame = 2;
        private IChannel _channel;

        public DefaultDotNettyServerMessageListener(ITransportMessageCodecFactory codecFactory,
            ILogger<DefaultDotNettyServerMessageListener> logger)
        {
            _transportMessageEncoder = codecFactory.GetEncoder();
            _transportMessageDecoder = codecFactory.GetDecoder();
            _logger = logger;
            Console.Write("create transport message for encoder and decoder.");
        }

        public event ReceivedDelegate Received;

        /// <summary>
        /// 触发接收到消息事件
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">接收到的消息</param>
        /// <returns>一个任务</returns>
        public async Task OnReceived(IMessageSender sender, TransportMessage message)
        {
            if (Received == null) return;
            Console.Write($"trigger 'OnReceived' event for message id {message.Id}");
            await Received(sender, message);
        }

        public async Task StartAsync(EndPoint endPoint)
        {
            _logger.LogInformation($"准备启动服务主机，监听地址：{endPoint}");

            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new LengthFieldPrepender(_lengthFieldPrepender));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, _basedFrame, 0, _basedFrame));
                    pipeline.AddLast(new TransportMessageChannelHandlerDecodeAdapter(_transportMessageDecoder));
                    pipeline.AddLast(new TransportMessageChannelHandlerEncodeAdapter(async (contenxt, message) =>
                        {
                            var sender = new DefaultDotNettyServerMessageSender(_transportMessageEncoder, contenxt);
                            await OnReceived(sender, message);
                        },
                        _logger));
                }));
            _channel = await bootstrap.BindAsync(endPoint);
            _logger.LogInformation($"服务主机启动成功，监听地址：{endPoint}");
        }

        public void Dispose()
        {
            Task.Run(async () => { await _channel.DisconnectAsync(); }).Wait();
        }
    }
}