using Easy.Rpc.Core.Communally.Entitys.Messages;

namespace Easy.Rpc.Transport.Codec
{
    public interface ITransportMessageDecoder
    {
        TransportMessage Decode(byte[] data);
    }
}