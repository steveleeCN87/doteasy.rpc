using DotEasy.Rpc.Core.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Transport.Codec
{
    public interface ITransportMessageDecoder
    {
        TransportMessage Decode(byte[] data);
    }
}