using System.Threading.Tasks;
using DotEasy.Rpc.Core.Communally.Entitys.Messages;
using DotEasy.Rpc.Transport.Codec;
using DotNetty.Transport.Channels;

namespace DotEasy.Rpc.Transport.Impl
{
    /// <summary>
    /// 基于DotNetty服务端的消息发送者
    /// </summary>
    public class DefaultDotNettyServerMessageSender : DefaultDotNettyMessageSender, IMessageSender
    {
        private readonly IChannelHandlerContext _context;

        public DefaultDotNettyServerMessageSender(ITransportMessageEncoder transportMessageEncoder,
            IChannelHandlerContext context) : base(transportMessageEncoder)
        {
            _context = context;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>一个任务</returns>
        public Task SendAsync(TransportMessage message)
        {
            var buffer = GetByteBuffer(message);
            return _context.WriteAsync(buffer);
        }

        /// <summary>
        /// 发送消息并清空缓冲区
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>一个任务</returns>
        public Task SendAndFlushAsync(TransportMessage message)
        {
            var buffer = GetByteBuffer(message);
            return _context.WriteAndFlushAsync(buffer);
        }
    }
}