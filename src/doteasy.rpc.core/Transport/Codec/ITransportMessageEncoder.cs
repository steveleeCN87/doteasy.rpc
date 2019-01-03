using DotEasy.Rpc.Runtime.Communally.Entitys.Messages;

namespace DotEasy.Rpc.Transport.Codec
{
    public interface ITransportMessageEncoder
    {
        byte[] Encode(TransportMessage message);
    }
}