using Easy.Rpc.Core.Communally.Entitys.Messages;

namespace Easy.Rpc.Transport.Codec
{
    public interface ITransportMessageEncoder
    {
        byte[] Encode(TransportMessage message);
    }
}