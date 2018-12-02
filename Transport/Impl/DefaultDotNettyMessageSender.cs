using DotEasy.Rpc.Core.Communally.Entitys.Messages;
using DotEasy.Rpc.Transport.Codec;
using DotNetty.Buffers;

namespace DotEasy.Rpc.Transport.Impl
{
    /// <summary>
    /// 基于DotNetty的消息发送者基类
    /// </summary>
    public abstract class DefaultDotNettyMessageSender
    {
        private readonly ITransportMessageEncoder _transportMessageEncoder;

        protected DefaultDotNettyMessageSender(ITransportMessageEncoder transportMessageEncoder)
        {
            _transportMessageEncoder = transportMessageEncoder;
        }

        protected IByteBuffer GetByteBuffer(TransportMessage message)
        {
            var data = _transportMessageEncoder.Encode(message);

            var buffer = Unpooled.Buffer(data.Length, data.Length);
            return buffer.WriteBytes(data);
        }
    }
}