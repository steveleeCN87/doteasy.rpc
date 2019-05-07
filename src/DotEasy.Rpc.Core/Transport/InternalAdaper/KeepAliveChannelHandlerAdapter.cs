using System;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;

namespace DotEasy.Rpc.Core.Transport.InternalAdaper
{
    /// <summary>
    /// 长连接适配器
    /// </summary>
    /// <remarks>
    /// dotnettey默认5秒做一次心跳检测，一次心跳检测就会触发ChannelActive和ChannelInactive函数，当远程连接断开后，dotnetty会调用ExceptionCaught函数
    /// </remarks>
    public class KeepAliveChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly ILogger _logger;

        public KeepAliveChannelHandlerAdapter(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Channel Inactive
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>确定远程连接存在后自动断开</remarks>
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _logger.LogInformation(context.Name);
            base.ChannelInactive(context);
        }

        /// <summary>
        /// Channel Active
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>确定远程连接存在</remarks>
        public override void ChannelActive(IChannelHandlerContext context)
        {
            _logger.LogInformation(context.Name);
            base.ChannelActive(context);
        }

        /// <summary>
        /// exception happened
        /// </summary>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
//            _logger.LogError($"与远程服务器：{context.Channel.RemoteAddress} 通信时发送了错误 \r{exception}");
            Console.WriteLine($@"与远程服务器：{context.Channel.RemoteAddress} 通信时发送了错误 \r{exception}");
            context.CloseAsync();
            base.ExceptionCaught(context, exception);
        }
    }
}