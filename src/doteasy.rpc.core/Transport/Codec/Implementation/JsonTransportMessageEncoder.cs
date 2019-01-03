using System.Text;
using DotEasy.Rpc.Runtime.Communally.Entitys.Messages;
using Newtonsoft.Json;

namespace DotEasy.Rpc.Transport.Codec.Implementation
{
    public sealed class JsonTransportMessageEncoder : ITransportMessageEncoder
    {
        public byte[] Encode(TransportMessage message)
        {
            var content = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(content);
        }
    }
}