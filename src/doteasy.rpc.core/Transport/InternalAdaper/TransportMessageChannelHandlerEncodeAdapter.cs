using System;
using System.Threading.Tasks;
using DotEasy.Rpc.Core.Runtime.Communally.Entitys.Messages;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Core.Transport.InternalAdaper
{
    /// <summary>
    /// 默认通道的解码器，也是默认通道状态监测器
    /// </summary>
    public class TransportMessageChannelHandlerEncodeAdapter : ChannelHandlerAdapter
    {
        private readonly Action<IChannelHandlerContext, TransportMessage> _readAction;
        private readonly ILogger _logger;

        public TransportMessageChannelHandlerEncodeAdapter(Action<IChannelHandlerContext, TransportMessage> readAction,
            ILogger logger)
        {
            _readAction = readAction;
            _logger = logger;
        }

        /// <summary>
        /// received messages
        /// </summary>
        public override void ChannelRead(IChannelHandlerContext context, object message) =>
            Task.Run(() => _readAction(context, (TransportMessage) message));

        /// <summary>
        /// read complete happened
        /// </summary>
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        /// <summary>
        /// exception happened
        /// </summary>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) =>
            _logger.LogError($"与远程服务器：{context.Channel.RemoteAddress} 通信时发送了错误 \r{exception}");
    }
}