using System.Text;
using DotEasy.Rpc.Core.Communally.Entitys.Messages;
using Newtonsoft.Json;

namespace DotEasy.Rpc.Transport.Codec.Implementation
{
    public sealed class JsonTransportMessageDecoder : ITransportMessageDecoder
    {
        public TransportMessage Decode(byte[] data)
        {
            var content = Encoding.UTF8.GetString(data);
            var message = JsonConvert.DeserializeObject<TransportMessage>(content);
            if (message.IsInvokeMessage())
            {
                message.Content = JsonConvert.DeserializeObject<RemoteInvokeMessage>(message.Content.ToString());
            }
            if (message.IsInvokeResultMessage())
            {
                message.Content = JsonConvert.DeserializeObject<RemoteInvokeResultMessage>(message.Content.ToString());
            }
            return message;
        }
    }
}