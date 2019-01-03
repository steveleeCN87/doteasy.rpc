using DotEasy.Rpc.Core.Runtime.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Core.Transport.Codec
{
    public interface ITransportMessageDecoder
    {
        TransportMessage Decode(byte[] data);
    }
}