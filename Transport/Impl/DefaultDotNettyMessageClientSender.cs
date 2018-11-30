using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Easy.Rpc.Core.Communally.Entitys.Messages;
using Easy.Rpc.Transport.Codec;

namespace Easy.Rpc.Transport.Impl
{
    /// <summary>
    /// 基于DotNetty客户端的消息发送者
    /// </summary>
    public class DefaultDotNettyMessageClientSender : DefaultDotNettyMessageSender, IMessageSender, IDisposable
    {
        private readonly IChannel _channel;

        public DefaultDotNettyMessageClientSender(ITransportMessageEncoder transportMessageEncoder, IChannel channel) :
            base(transportMessageEncoder)
        {
            _channel = channel;
        }


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Task.Run(async () => { await _channel.DisconnectAsync(); }).Wait();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>一个任务</returns>
        public async Task SendAsync(TransportMessage message)
        {
            var buffer = GetByteBuffer(message);
            await _channel.WriteAsync(buffer);
        }

        /// <summary>
        /// 发送消息并清空缓冲区
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>一个任务</returns>
        public async Task SendAndFlushAsync(TransportMessage message)
        {
            var buffer = GetByteBuffer(message);
            await _channel.WriteAndFlushAsync(buffer);
        }
    }
}