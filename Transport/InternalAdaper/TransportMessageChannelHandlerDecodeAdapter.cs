using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Easy.Rpc.Transport.Codec;

namespace Easy.Rpc.Transport.InternalAdaper
{
    /// <summary>
    /// 标准通道的编码适配器
    /// </summary>
    internal class TransportMessageChannelHandlerDecodeAdapter : ChannelHandlerAdapter
    {
        private readonly ITransportMessageDecoder _transportMessageDecoder;

        public TransportMessageChannelHandlerDecodeAdapter(ITransportMessageDecoder transportMessageDecoder)
        {
            _transportMessageDecoder = transportMessageDecoder;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer) message;
            var data = buffer.ToArray();
            if (data.Length == 0) return;
            var transportMessage = _transportMessageDecoder.Decode(data);
            context.FireChannelRead(transportMessage);
        }
    }
}