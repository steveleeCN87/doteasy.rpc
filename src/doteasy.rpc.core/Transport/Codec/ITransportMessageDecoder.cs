using DotEasy.Rpc.Runtime.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Transport.Codec
{
    public interface ITransportMessageDecoder
    {
        TransportMessage Decode(byte[] data);
    }
}